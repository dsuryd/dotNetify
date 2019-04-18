
<p align="center"><img width="400px" src="http://dotnetify.net/content/images/dotnetify-logo.png"></p>

![alt build](https://ci.appveyor.com/api/projects/status/github/dsuryd/dotnetify?svg=true)
[![npm version](https://badge.fury.io/js/dotnetify.svg)](https://badge.fury.io/js/dotnetify)
[![NuGet](https://img.shields.io/nuget/v/DotNetify.SignalR.svg?style=flat-square)](https://www.nuget.org/packages/DotNetify.SignalR/) 

DotNetify is a free, open source project that lets you create real-time, reactive, cross-platform apps with [React](https://facebook.github.io/react/), [React Native](https://facebook.github.io/react-native/), [Vue](https://vuejs.org) or [Knockout](http://knockoutjs.com) front-end on C# .NET back-end via WebSocket. 

## What's New

* v3.3: **.NET client support - [release notes](https://github.com/dsuryd/dotNetify/releases/tag/v3.3.1).**
* v3.2: integrates with Vue.
* v3.1: [release notes](https://github.com/dsuryd/dotNetify/releases/tag/nuget_v.3.1).
* v3.0: integrates with SignalR for ASP.NET Core 2.1.
<br/><br/>
* **DotNetify-Elements** is out! <a href="http://dotnetify.net/elements">See documentation and live demo.</a> 
* React SPA template for ASP.NET Core 2.1 - [github](https://github.com/dsuryd/dotnetify-react-demo-vs2017/tree/master/ReactTemplate).

## Features

* Simple and lightweight - no heavy JS client-side framework, and no REST APIs.
* Reactive back-end MVVM architecture on both .NET Core and .NET Framework.
* Built-in real-time across WebSocket with SignalR.
* Full support for single-page apps, including deep-linked, nested routing and token-based authentication.
* Powerful back-end infrastructure, including dependency injection, WebSocket request/response pipelines, and modern tooling like VS2017 and Webpack.

## Documentation

Documentation and live demo can be found at [https://dotnetify.net](http://dotnetify.net).

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

*PM> Install-Package DotNetify.Core*  
*PM> Install-Package DotNetify.SignalR*  

ASP.NET Framework:

*PM> Install-Package DotNetify.Core*  
*PM> Install-Package DotNetify.SignalR.Owin*  

Read the website on how to configure your project.

## License
Licensed under the Apache License, Version 2.0.

## Contributing
All contribution is welcome: star this project, let others know about it, report issues, submit pull requests!

_Logo design by [area55git](https://github.com/area55git)._
