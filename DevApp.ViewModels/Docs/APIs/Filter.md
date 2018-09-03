## View Model Filter

View model filters are similar to middlewares in which they too have access to raw client requests before they are resolved by the view models. However, they are only executed at the back-end of the request pipeline, when the view model instance associated with the request has been identified, so they can be given the opportunity to interact directly with the view model.

A view model filter is always paired with an attribute class. The attribute serves to identify which view model type that the filter targets, and can also be provided with arguments for the filter.

To implement a view model filter, create both an attribute and a class that inherits from __IVMFilter<T>__ where _T_ is the attribute type. For example:

```csharp
[AttributeUsage(AttributeTargets.Class)]
public class AuthorizeAttribute : Attribute {}

public class AuthorizeFilter : IVMFilter<AuthorizeAttribute>
{
   public Task Invoke(AuthorizeAttribute auth, VMContext context, NextFilterDelegate next)
   {
      if (context.HubContext.Principal?.Identity?.IsAuthenticated == false)
         throw new UnauthorizedAccessException();

      return next(context);
   }
}
```

Use the filter on a view model by adorning the class with the attribute. When the client request is received, the filter can either pass the control to the next filter in the pipeline until it reaches the view model, halt the execution by not invoking the next delegate, or throw an exception.

#### Filter Registration

To register the filter to the pipeline, add it to dotNetify's configuration with __UseFilter<T>__.

```csharp
public void Configuration(IAppBuilder app)
{
   ...
   app.UseDotNetify(config => {
      config.UseFilter<AuthorizeFilter>();
   });
}
```