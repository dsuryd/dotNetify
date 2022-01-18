using DotNetify;
using StockTicker;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDotNetify().AddSignalR();
builder.Services.AddScoped<IStockTickerService, StockTickerService>();

var app = builder.Build();

app.MapHub<DotNetifyHub>("/dotnetify");

app.MapVM("StockTicker", (IStockTickerService service) => new
{
   service.StockPrices,
   AddSymbol = new Command<string>(symbol => service.AddSymbol(symbol))
});

app.UseStaticFiles();
app.MapFallbackToFile("index.html");

app.Run();