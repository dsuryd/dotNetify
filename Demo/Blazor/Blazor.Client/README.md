NOTES:

DotNetify uses JSON.NET to deserialize incoming server messages.  This comes with an issue as reported in https://github.com/aspnet/Blazor/issues/370.
The workaround is to disable BlazorLinkOnBuild in the .csproj: https://github.com/aspnet/Blazor/issues/370#issuecomment-409636546
