# &nbsp;![alt tag](http://dotnetify.net/content/images/greendot.png) dotNetify 
![alt build](https://ci.appveyor.com/api/projects/status/github/dsuryd/dotnetify?svg=true)

DotNetify is a free, open source project that lets you create amazing real-time, reactive web apps with HTML/Javascript front-end on cross-platform C# .NET back-end. 


__*** dotNetify-React is in BETA - check out docs and live demo at [http://dotnetify.net/react](http://dotnetify.net/react)__

## Features

* Simple and lightweight - no big JS client-side framework, no REST APIs, no explicit HTTP requests.
* Integrate SignalR and [React](https://facebook.github.io/react/), or Knockout to support reactive .NET back-end MVVM.
* Built-in real-time across WebSocket, perfect for IoT consumer apps.
* Full support for single-page apps, including deep-linked, nested routing.
* Run on [ASP.NET Core](http://asp.net/core), ASP.NET Framework and [Nancy](https://github.com/dsuryd/dotNetify-Nancy-demo).
* Modern tooling: Visual Studio 2017,  [Typescript](https://www.typescriptlang.org/), [NPM](https://www.npmjs.com/), [WebPack](https://webpack.github.io/).
* __NEW:__ Powerful infrastructure that includes token-based authentication support, dependency injections, WebSocket request/response pipelines.

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

*PM> Install-Package DotNetify.Core -pre*  
*PM> Install-Package DotNetify.SignalR -pre*  
*$ npm install dotnetify --save*

Read the website on how to configure your project ([React](http://dotnetify.net/react/Installation) | [Knockout](http://dotnetify.net/index/Installing)).

## License
Licensed under the Apache License, Version 2.0.

## Contributing
All contribution is welcome; reach out to find out how you can help.  If you like the idea behind this project, please let others know about it! 
