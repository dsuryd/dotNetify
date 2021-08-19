## Real-time PostgreSQL

You can use dotNetify to subscribe to data change events made to a PostgreSQL database with _logical replication_ and push them to your web application in real-time.

Logical replication is a PostgreSQL's feature for ensuring copies of the database are always in sync by having the database publish logical data changes to subscriber nodes. The subscriber is typically another database that is serving as a copy to the master, but in our case, we make it to be a dotNetify server that can in turn broadcast changes to its web clients.

Compared to NOTIFY command, logical replication has a couple of key advantages that makes it more scalable: it doesn't require us writing database triggers for each table we're interested in, and the payload has no size limit.

#### PostgreSQL Setup

To enable logical replication in your PostgreSQL database, find the _postgresql.conf_ configuration file, change the parameter `wal_level` to _logical_, and both `max_wal_senders` and `max_replication_slots` to at least 1. Changes will take effect after the service restarts.

You can also change them with SQL commands:

```sql
ALTER SYSTEM SET wal_level='logical';
ALTER SYSTEM SET max_wal_senders='10';
ALTER SYSTEM SET max_replication_slots='10';
```

The next step is to create a publication:

```sql
CREATE PUBLICATION my_pub FOR ALL TABLES;
```

We set it to publish data changes for all tables, but you can restrict it to just a specific table if you want.

When PostgreSQL is publishing replication records (also known as _write-ahead logs_ or WAL), it uses something called replication slots to ensure that the records do not get deleted until they're received by the subscribers.

Replication slots are great because they allow a subscriber to go temporarily offline and, on reconnect, to simply pick up where it left off. But there's a caveat: the WAL records can pile up in a prolonged disconnection event, to the point that it can run out of space and crash the database, and therefore, the slots need to be monitored.

Here's how we create a replication slot:

```sql
SELECT * FROM pg_create_logical_replication_slot('my_slot', 'pgoutput');
```

The _pgoutput_ is PostgreSQL's standard logical decoding plugin for transforming the changes from WAL to the logical replication protocol.

You need to give the REPLICATION role to the user account that the dotNetify server will use to connect. Having this role is required for subscribing to replication slots.

#### DotNetify Server Setup

Add **DotNetify.Postgres** NuGet library to your project, then provide the PostgreSQL information in the service startup:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddSignalR();
    services.AddDotNetify();
    services.AddDotNetifyPostgres(new PostgresConfiguration
    {
      ConnectionString = Configuration.GetConnectionString("Postgres"),
      PublicationName = "my_pub",
      ReplicationSlotName = "my_slot"
    });
}
```

Define an entity class for the database table you want to observe. For example:

```csharp
[Table("businesses")]
public class Business
{
  [Column("business_id")]
  [Key]
  public long Id { get; set; }

  [Column("business_name")]
  public string Name { get; set; }

  [Column("rating")]
  public int Rating { get; set; }
}
```

Inject **IDbChangeObserver** interface to your view model and use it to subscribe to the data change events on the table. Combine this with the [CRUD APIs](/core/api/crud) to push updates to the web clients. For example:

```csharp
public class BusinessesVM : BaseVM
{
  private IDisposable _subs;

  [ItemKey(nameof(Business.Id))]
  public List<Business> Businesses { get; set; } = new List<Business>();

  public BusinessesVM(IDbChangeObserver dbChangeObserver)
  {
    _subs = dbChangeObserver
      .Observe<Business>()
      .Subscribe(e =>
      {
          if (e is DbInsertEvent<Business>)
          {
            var row = (e as DbInsertEvent<Business>).Row;
            this.AddList(nameof(Businesses), row);
          }
          else if (e is DbUpdateEvent<Business>)
          {
            var row = (e as DbUpdateEvent<Business>).NewRow;
            this.UpdateList(nameof(Businesses), row);
          }
          else if (e is DbDeleteEvent<Business>)
          {
            var key = (e as DbDeleteEvent<Business>).Row.Id;
            this.RemoveList(nameof(Businesses), key);
          }
          PushUpdates();
    });
  }

  public override void Dispose() => _subs.Dispose();
}
```

There's also an extension method **ObserveList** to make the above code more concise:

```csharp
public class BusinessesVM : BaseVM
{
  [ItemKey(nameof(Business.Id))]
  public List<Business> Businesses { get; set; } = new List<Business>();

  public BusinessesVM(IDbChangeObserver dbChangeObserver)
  {
      this.ObserveList<Business>(nameof(Businesses), dbChangeObserver);
  }
}
```

#### Full CRUD with EF Core

Combined with EF Core and [Npgsql](https://www.npgsql.org/) library, you can make the view model support all the CRUD operations.

Add a `DbContext` class for the table:

```csharp
public class BusinessDbContext : DbContext
{
  public DbSet<Business> Businesses { get; set; }
  public BusinessDbContext(DbContextOptions<BusinessDbContext> options)
    : base(options) {}
}
```

DotNetify view models must use a factory to create a new `DbContext` because of the long lifetimes. So we configure the `DbContextFactory` service in the startup class:

```csharp
public void ConfigureServices(IServiceCollection services)
{
  ...
  services.AddDbContextFactory<BusinessDbContext>(options =>
    options.UseNpgsql(Configuration.GetConnectionString("Postgres")));
}
```

Then implement the CRUD methods on your view model. For example:

```csharp
public class BusinessesVM : BaseVM
{
  private readonly IDbContextFactory<BusinessDbContext> _contextFactory;

  [ItemKey(nameof(Business.Id))]
  public List<Business> Businesses { get; set; }

  public BusinessesVM(
    IDbContextFactory<BusinessDbContext> dbContextFactory,
    IDbChangeObserver dbChangeObserver)
  {
      _contextFactory = dbContextFactory;

      using var dbContext = _contextFactory.CreateDbContext();
      Businesses = dbContext.Businesses.OrderBy(x => x.Id).ToList();

      this.ObserveList<Business>(nameof(Businesses), dbChangeObserver);
  }

  public void Add(Business businessInfo)
  {
      using var dbContext = _contextFactory.CreateDbContext();
      dbContext.Businesses.Add(businessInfo);
      dbContext.SaveChanges();
  }

  public void Update(Business businessInfo)
  {
      using var dbContext = _contextFactory.CreateDbContext();
      var business = dbContext.Businesses.Find(businessInfo.Id);
      if (business != null)
      {
        business.Name = businessInfo.Name;
        business.Rating = businessInfo.Rating;
        dbContext.SaveChanges();
      }
  }

  public void Remove(Business businessInfo)
  {
      using var dbContext = _contextFactory.CreateDbContext();
      var business = dbContext.Businesses.Find(businessInfo.Id);
      if (business != null)
      {
        dbContext.Businesses.Remove(business);
        dbContext.SaveChanges();
      }
  }
}
```
