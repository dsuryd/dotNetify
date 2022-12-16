#define AWS_INTEGRATION

using System.Text;
using Amazon.Runtime;
using AwsSignatureVersion4;
using DotInitializr;
using DotNetify;
using DotNetify.DevApp;
using DotNetify.Pulse;
using DotNetify.Security;
using DotNetify.WebApi;
using Jering.Javascript.NodeJS;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

// Add OpenID Connect server to produce JWT access tokens.
services.AddAuthenticationServer();

services.AddSignalR()
   //.AddMessagePackProtocol()
   ;
services.AddDotNetify();
services.AddDotNetifyPulse();
services.AddDotInitializr();
services.AddMvc();

services.AddScoped<IEmployeeRepository, EmployeeRepository>();
services.AddSingleton<IMovieService, MovieService>();
services.AddSingleton<IWebStoreService, WebStoreService>();

StaticNodeJSService.Configure<OutOfProcessNodeJSServiceOptions>(options => options.TimeoutMS = 2000);

#if AWS_INTEGRATION

var awsCredentials = new ImmutableCredentials(builder.Configuration["Aws:AccessKeyId"], builder.Configuration["Aws:SecretAccessKey"], null);
services
  .AddTransient<AwsSignatureHandler>()
  .AddTransient(_ => new AwsSignatureHandlerSettings(builder.Configuration["Aws:Region"], "execute-api", awsCredentials))
  .AddHttpClient<DotNetifyWebApi>(client => { client.BaseAddress = new Uri(@builder.Configuration["Aws:ConnectionUrl"]); })
  .AddHttpMessageHandler<AwsSignatureHandler>();

#endif

var app = builder.Build();

app.UseAuthentication();
app.UseWebSockets();
app.UseDotNetify(config =>
{
   config.RegisterAssembly("DotNetify.DevApp.ViewModels");

   var tokenValidationParameters = new TokenValidationParameters
   {
      IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(AuthServer.SecretKey)),
      ValidateIssuerSigningKey = true,
      ValidateAudience = false,
      ValidateIssuer = false,
      ValidateLifetime = true,
      ClockSkew = TimeSpan.FromSeconds(0)
   };

   // Middleware to do authenticate token in incoming request headers.
   config.UseJwtBearerAuthentication(tokenValidationParameters);

   // Filter to check whether user has permission to access view models with [Authorize] attribute.
   config.UseFilter<AuthorizeFilter>();

   // Middleware to log incoming/outgoing message; default to Sytem.Diagnostic.Trace.
   var logger = app.Services.GetService<ILogger<VMController>>();
   config.UseDeveloperLogging(log =>
   {
      if (!log.Contains("vmId=PulseVM"))
         logger.LogInformation(log);
      System.Diagnostics.Trace.WriteLine(log);
   });

   // Demonstration middleware that extracts auth token from incoming request headers.
   config.UseMiddleware<ExtractAccessTokenMiddleware>(tokenValidationParameters);

   // Demonstration filter that passes access token from the middleware to the ViewModels.SecurePageVM class instance.
   config.UseFilter<SetAccessTokenFilter>();
});
app.UseDotNetifyPulse();

if (app.Environment.IsDevelopment())
{
#pragma warning disable 618
   app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
   {
      HotModuleReplacement = true,
      HotModuleReplacementClientOptions = new Dictionary<string, string> { { "reload", "true" } },
   });
#pragma warning restore 618
}

app.UseStaticFiles();
app.UseRouting();
app.MapHub<DotNetifyHub>("/dotnetify");
app.MapControllers();

//app.UseSsr(typeof(App), (string[] args) => StaticNodeJSService.InvokeFromFileAsync<string>("wwwroot/ssr", null, args), DefaultRequestHandler);

app.MapFallbackToFile("index.html");
app.Run();