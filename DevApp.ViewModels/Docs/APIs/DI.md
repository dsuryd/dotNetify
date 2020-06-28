## Dependency Injection

DotNetify is integrated with the built-in ASP.NET DI container. Service registration is typically done in the _ConfigureServices_ method of the application's _Startup_ class. For example:

```csharp
public void ConfigureServices(IServiceCollection services)
{
   services.AddSignalR();
   services.AddDotNetify();

   services.AddTransient<IServiceA, ServiceA>();
   services.AddSingleton<IServiceB, ServiceB>();
}
```

When instantiating view models, dotNetify uses the _ActivatorUtilities_ class from _Microsoft.Extensions.DependencyInjection_, which will automatically resolve any dependency to registered services. This class is also capable of injecting concrete types without the need for explicit registration.

```csharp
public class MyViewModel : BaseVM
{
   private readonly IServiceA _serviceA;
   private readonly IServiceB _serviceB;

   public MyViewModel(IServiceA serviceA, IServiceB serviceB)
   {
       _serviceA = serviceA;
       _serviceB = serviceB;
   }
}
```

Services with scoped lifetime are created once per persistent connection.

#### Factory Method

If you need a greater control over dotNetify's object creation, you have the option to set up a custom factory method:

```csharp
app.UseDotNetify(config => {
   config.SetFactoryMethod((type, args) =>
   {
      /* return an instance of the specified type */
   });
});
```
