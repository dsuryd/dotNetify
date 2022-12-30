using Amazon.Runtime;
using AwsSignatureVersion4;
using BrunoLau.SpaServices.Webpack;
using DotNetify;
using DotNetify.WebApi;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

var awsCredentials = new ImmutableCredentials(config["Aws:AccessKeyId"], config["Aws:SecretAccessKey"], null);

builder.Services
    .AddTransient<AwsSignatureHandler>()
    .AddTransient(_ => new AwsSignatureHandlerSettings(config["Aws:Region"], "execute-api", awsCredentials))
    .AddDotNetifyIntegrationWebApi(client =>
      client.BaseAddress = new Uri(config["Aws:ConnectionUrl"])
    )
    .AddHttpMessageHandler<AwsSignatureHandler>();

builder.Services
  .AddDotNetifyResiliencyAddon()
  .AddStackExchangeRedisCache(options => options.Configuration = config["Redis:ConnectionString"]);

var app = builder.Build();

app.UseDotNetify();
app.UseDotNetifyResiliencyAddon();

if (app.Environment.IsDevelopment())
   app.UseWebpackDevMiddlewareEx(new WebpackDevMiddlewareOptions { HotModuleReplacement = true });

app.UseStaticFiles();
app.MapControllers();
app.MapFallbackToFile("index.html");
app.Run();