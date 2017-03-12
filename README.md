#&nbsp;![alt tag](http://dotnetify.net/content/images/greendot.png) dotNetify 
![alt build](https://ci.appveyor.com/api/projects/status/github/dsuryd/dotnetify?svg=true)

DotNetify is a free, open source project that lets you create amazing real-time web and mobile hybrid apps using HTML5 and C# on cross-platform .NET Core backend. 

__*** dotNetify-React is in BETA - check out docs and live demo at [http://dotnetify.net/react](http://dotnetify.net/react)__

##Features

* Simple and lightweight - no JS client-side framework, no REST API controllers.
* Integrate SignalR and [React](https://facebook.github.io/react/), or Knockout to support .NET back-end MVVM architecture.
* Built-in real-time across WebSockets, perfect for IoT consumer apps.
* Full support for single-page apps, including deep-linked, nested routing.
* Run on [ASP.NET Core](http://asp.net/core), ASP.NET 4.5 and [Nancy](https://github.com/dsuryd/dotNetify-Nancy-demo).
* Modern tooling: Visual Studio 2017,  [Typescript](https://www.typescriptlang.org/), [NPM](https://www.npmjs.com/), [WebPack](https://webpack.github.io/).

##Documentation

Documentation and live demo can be found at [http://dotnetify.net](http://dotnetify.net).

##React Code Example   

* Real-time "Hello World" with Visual Studio 2017 + WebPack: [dotnetify-react-demo-vs2017](https://github.com/dsuryd/dotnetify-react-demo-vs2017).   
* Real-time "Hello World" with create-react-app + Node.js + .NET Core: [dotnetify-react-demo](https://github.com/dsuryd/dotnetify-react-demo).  

##Knockout Code Example

* Real-time live chart: [Live Chart Example](https://github.com/dsuryd/dotNetify-example-livechart).    
* Mobile app example: [Web/Mobile App Template](https://github.com/dsuryd/dotNetify-app-template).

##Installation

ASP.NET 4.5:  
*PM> Install-Package DotNetify*

ASP.NET Core:  
*PM> Install-Package DotNetify.Core -pre*  
*PM> Install-Package DotNetify.SignalR -pre*  
*$ npm install dotnetify --save*

Read the website on how to configure your project ([React](http://dotnetify.net/react/Installation) | [Knockout](http://dotnetify.net/index/Installing)).

##License
Licensed under the Apache License, Version 2.0.

##Contributing
All contribution is welcome; reach out to find out how you can help.  If you like the idea behind this project, please let others know about it! 
