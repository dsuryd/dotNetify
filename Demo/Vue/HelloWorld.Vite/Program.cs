using DotNetify;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();
builder.Services.AddDotNetify(); 

if (builder.Environment.IsDevelopment())
{
   builder.Services.AddSpaYarp();
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
   app.UseSpaYarp();
}

app.UseWebSockets();
app.UseDotNetify();
app.MapHub<DotNetifyHub>("/dotnetify"); 

app.UseStaticFiles();
app.MapFallbackToFile("UI/index.html");

app.Run();
