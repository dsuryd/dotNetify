using DotNetify;
using StockTicker;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<IStockTickerService, StockTickerService>();

builder.Services.AddDotNetify().AddSignalR();

var app = builder.Build();

app.MapHub<DotNetifyHub>("/dotnetify");

app.MapVM("StockTicker", (IStockTickerService service) => new
{
   service.StockPrices,
   AddSymbol = new Action<string>(symbol => service.AddSymbol(symbol))
});

app.UseStaticFiles();
app.MapFallbackToFile("index.html");

app.Run();