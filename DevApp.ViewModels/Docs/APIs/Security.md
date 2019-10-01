## Security

#### ASP.NET Authentication

DotNetify relies on SignalR to integrate into the existing authentication structure, such as the ASP.NET Cookie Based Authentication. In a typical authentication process, the application website directs any unauthorized traffic to a login page. Once authenticated, the user is redirected to the page that uses dotNetify, which subsequently establishes persistent connection to the server within the authenticated user context.

To restrict access to your view models only to authenticated users, configure dotNetify to add the __AuthorizeFilter__:

```csharp
app.UseDotNetify(config => config.UseFilter<AuthorizeFilter>());
```

And then you can use the __Authorize__ attribute to prevent unauthenticated users from accessing any view model with the attribute:

```csharp
[Authorize]
public MyViewModel : BaseVM { /*...*/ }
```

When you need access to the authenticated user identity inside your view models in a .NET Framework environment, use _System.Threading.Thread.CurrentPrincipal_. If you are in .NET Standard or .NET Core environment, declare the interface __IPrincipalAccessor__ on your view model's constructor, and access the user identity from the injected object:

```csharp
public ContactList( IPrincipalAccessor principalAccessor )
{
   if (!Trace.Assert(principalAccessor.Principal.Identity.IsAuthenticated))
      throw new UserNotAuthenticatedException();
}
```

#### Token Based Authentication

Since SignalR doesn't support token-based authentication out of the box, dotNetify provides a mechanism to pass the JWT in the initial payload:

```jsx
this.vm = dotnetify.react.connect("SecurePage", this, {
   headers: { Authorization: "Bearer " + myAccessToken }
});
```

The __connect__ API's third parameter allows you to specify optional request headers, which the above code uses to set the _Authorization_ header using Bearer schema format. When the connection is established, the headers will be sent in the payload along with the view model request.

On the server side, you must configure dotNetify to use both the __AuthorizeFilter__ and the __JWTBearerAuthentication__ middleware:

```csharp
app.UseDotNetify(config =>
{
   var tokenValidationParameters = new TokenValidationParameters { /*...*/ }

   config.UseFilter<AuthorizeFilter>();
   config.UseJwtBearerAuthentication(tokenValidationParameters);
});
```

Inside the middleware, the token payload is extracted out of the headers, validated, and then resolved into the claims principal object, which then used by the __Authorize__ filter to check against the Authorize attribute properties declared on the view model.

The request headers are kept on the server and always go through the middleware pipeline so that any server-originated updates can also be authenticated against the token. When the token expires, the filter will throw an _UnauthorizedAccessException_ which will be sent back to the client (although you can always override this behavior through a custom middleware).

By default, dotNetify on the client side will just output any exception to the console, but you may wish to handle the exception so you can do things like displaying an error message or redirecting to a login page. In such case, add the handler function to the connect parameter:

```jsx
this.vm = dotnetify.react.connect("SecurePage", this, {
   headers: "{ Authorization: Bearer " + myAccessToken + " }",
   exceptionHandler: ex => this.onException(ex) 
});

this.onException = exception => {
   if (exception.name == "UnauthorizedAccessException")
      // ...redirect to login page
};
```

The exception object has name and message properties, respectively assigned to the server exception's type name and the exception message.

#### Authorization

The Authorize attribute lets you specify which claims or roles a view model is restricted to. For example:

```csharp
[Authorize(Role="Admin")]
public AdminViewModel : BaseVM { /*...*/ }

[Authorize(ClaimType="AddContact", ClaimValue="true")]
public AddContactViewModel : BaseVM { /*...*/ }
```

If you need more than this basic authorization, you can always write your own custom authorization attribute and filter.

#### IdentityServer4 Integration

If you use IdentityServer4 to generate the web token, build the JWT issuer signing keys by calling its API to get the security keys, and then convert the keys to the RSA format:
```csharp
using IdentityModel.Client;
...
public static async Task<IEnumerable<SecurityKey>> GetIssuerSigningKeysAsync(HttpClient client, string id4ApiUri)
{
   var disco = await client.GetDiscoveryDocumentAsync(id4ApiUri);
   var keys = new List<SecurityKey>();
   foreach(var webKey in disco.KeySet.Keys)
   {
      var key = new RsaSecurityKey(new RSAParameters
      {
         Exponent = Base64Url.Decode(webKey.E),
         Modulus = Base64Url.Decode(webKey.N)
      });

      key.KeyId = webKey.Kid;
      keys.Add(key);
   }
   return keys;
}
```