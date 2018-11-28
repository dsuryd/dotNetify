<p ><img width="200px" src="http://dotnetify.net/content/images/dotnetify-logo.png"></p>

This solution demonstrates a WPF desktop app, an Avalonia desktop app, and a Vue webpage communicating in real-time with a C# view model running on an ASP.NET Core server.

<img src="http://dotnetify.net/content/images/Wpf_Avalonia_Demo.gif" />

### How to run

- Open `HelloWorld.sln` from Visual Studio
- Open _Project -> Set Startup Projects_ menu.
- Select _Multiple Startup Projects_ and set `AvaloniaClient`, `Server`, `WpfClient` to _Start_.
- Start (F5).

_Note: the server port is set to 5000 in launchSettings.json.  If changed, update the client's settings in Bootstrap.cs._
