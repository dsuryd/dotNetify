# &nbsp;![alt tag](http://dotnetify.net/content/images/greendot.png) dotNetify 
![alt build](https://ci.appveyor.com/api/projects/status/github/dsuryd/dotnetify?svg=true)
[![npm version](https://badge.fury.io/js/dotnetify.svg)](https://badge.fury.io/js/dotnetify)
[![NuGet](https://img.shields.io/nuget/v/DotNetify.SignalR.svg?style=flat-square)](https://www.nuget.org/packages/DotNetify.SignalR/) 

DotNetify is a free, open source project that lets you create real-time, reactive, cross-platform apps with [React](https://facebook.github.io/react/), [React Native](https://facebook.github.io/react-native/), or [Knockout](http://knockoutjs.com) front-end on C# .NET back-end via WebSocket. 

## What's New

* npm/nuget v3.0.0-pre: integrates with ***SignalR .NET Core v1.0.0-preview2-final*** - [release notes](https://github.com/dsuryd/dotNetify/releases/tag/v3.0.0-pre)
* nuget v2.3.0-pre: ***Reactive programming support*** - [release notes](https://github.com/dsuryd/dotNetify/releases/tag/nuget_v.2.3.0-pre) | [documentation](http://www.dotnetify.net/react/Reactive).
* React SPA template for .NET Core 2.0 - [github](https://github.com/dsuryd/dotnetify-react-demo-vs2017/tree/master/ReactTemplate).

> While you're here, also check out the companion project [dotNetify-Elements](https://github.com/dsuryd/dotNetify-Elements) (work in progress).

## Features

* Simple and lightweight - no heavy JS client-side framework, and no REST APIs.
* Reactive back-end MVVM architecture on both .NET Core and .NET Framework.
* Built-in real-time across WebSocket with SignalR.
* Full support for single-page apps, including deep-linked, nested routing and token-based authentication.
* Powerful back-end infrastructure, including dependency injection, WebSocket request/response pipelines, and modern tooling like VS2017 and Webpack.

## Documentation

Documentation and live demo can be found at [http://dotnetify.net](http://dotnetify.net).

## React/React Native Code Example   

* Real-time "Hello World" with Visual Studio 2017 + WebPack: [dotnetify-react-demo-vs2017](https://github.com/dsuryd/dotnetify-react-demo-vs2017).   
* Real-time "Hello World" with create-react-app + Node.js + .NET Core: [dotnetify-react-demo](https://github.com/dsuryd/dotnetify-react-demo).  Includes example using ***Electron*** to build cross-platform desktop app.
* React Native example: [dotnetify-react-native-demo](https://github.com/dsuryd/dotnetify-react-native-demo).

## Knockout Code Example

* Live chart: [Live Chart Example](https://github.com/dsuryd/dotnetify-knockout-demo/tree/master/LiveChart).    
* Mobile app example: [Web/Mobile App Template](https://github.com/dsuryd/dotnetify-knockout-demo/tree/master/MobileApp).

## Installation

*$ npm install dotnetify --save*

ASP.NET Core:

*PM> Install-Package DotNetify.Core -pre*  
*PM> Install-Package DotNetify.SignalR -pre*  

ASP.NET Framework:

*PM> Install-Package DotNetify.Core -pre*  
*PM> Install-Package DotNetify.SignalR.Owin -pre*  

Read the website on how to configure your project ([React](http://dotnetify.net/react/Installation) | [Knockout](http://dotnetify.net/index/Installing)).

## License
Licensed under the Apache License, Version 2.0.

## Contributing
All contribution is welcome: star this project, let others know about it, report issues, submit pull requests!
