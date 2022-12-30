using DotNetify;
using BrunoLau.SpaServices.Webpack;

var builder = WebApplication.CreateBuilder(args);
         
builder.Services.AddSignalR();
builder.Services.AddDotNetify(); 

var app = builder.Build();

if (app.Environment.IsDevelopment())
  app.UseWebpackDevMiddlewareEx(new WebpackDevMiddlewareOptions { HotModuleReplacement = true });

app.UseDotNetify();

app.MapHub<DotNetifyHub>("/dotnetify"); 
app.MapFallbackToFile("index.html");

app.Run();