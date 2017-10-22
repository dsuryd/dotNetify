# &nbsp;![alt tag](http://dotnetify.net/content/images/greendot.png) dotNetify 
![alt build](https://ci.appveyor.com/api/projects/status/github/dsuryd/dotnetify?svg=true)
[![npm version](https://badge.fury.io/js/dotnetify.svg)](https://badge.fury.io/js/dotnetify)

DotNetify is a free, open source project that lets you create real-time, reactive, cross-platform apps with [React](https://facebook.github.io/react/), [React Native](https://facebook.github.io/react-native/), or [Knockout](http://knockoutjs.com) front-end on C# .NET back-end via WebSocket. 

## What's New

* Latest: ***integrates with SignalR .NET Core 2.0 (alpha2)*** - [release notes](https://github.com/dsuryd/dotNetify/releases/tag/v2.0.7-beta%2F2.2.2-pre).
* v2.0.6-beta: supports React Native - [see demo](https://github.com/dsuryd/dotnetify-react-native-demo).

## Features

* Simple and lightweight - no heavy JS client-side framework, and no REST APIs.
* Reactive back-end MVVM architecture on both .NET Core and .NET Framework.
* Built-in real-time across WebSocket with SignalR.
* Full support for single-page apps, including deep-linked, nested routing and token-based authentication.
* Powerful back-end infrastructure, including dependency injection, WebSocket request/response pipelines, and modern tooling like VS2017 and Webpack.

## Documentation

Documentation and live demo can be found at [http://dotnetify.net](http://dotnetify.net).

## React Code Example   

* Real-time "Hello World" with Visual Studio 2017 + WebPack: [dotnetify-react-demo-vs2017](https://github.com/dsuryd/dotnetify-react-demo-vs2017).   
* Real-time "Hello World" with create-react-app + Node.js + .NET Core: [dotnetify-react-demo](https://github.com/dsuryd/dotnetify-react-demo).  
Includes example using ***Electron*** to build cross-platform desktop app.

## Knockout Code Example

* Real-time live chart: [Live Chart Example](https://github.com/dsuryd/dotnetify-knockout-demo/tree/master/LiveChart).    
* Mobile app example: [Web/Mobile App Template](https://github.com/dsuryd/dotnetify-knockout-demo/tree/master/MobileApp).

## Installation

*$ npm install dotnetify --save*

ASP.NET Core:

*PM> Install-Package DotNetify.Core -pre*  
*PM> Install-Package DotNetify.SignalR -pre*  

ASP.NET Framework:

*PM> Install-Package DotNetify*  

Read the website on how to configure your project ([React](http://dotnetify.net/react/Installation) | [Knockout](http://dotnetify.net/index/Installing)).

## License
Licensed under the Apache License, Version 2.0.

## Contributing
All contribution is welcome: star this project, let others know about it, report issues, submit pull requests!
