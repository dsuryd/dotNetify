## Get Started

The easiest way to get started is to perform the following step-by-step instructions given in the _Overview_ to create a new project from scratch with either WebPack or script tag.  
[inset]

#### Client-Side Library

If you use module bundler like WebPack, install __dotnetify__ and `knockout` from NPM: 
```js
npm i --save dotnetify
npm i --save knockout
```

If you write code-behind classes, every class must be added to the global _window_ variable.  For example:
```jsx
import HelloWorld from './HelloWorld.ts';
window.HelloWorld = HelloWorld;
```


If using script tags, include _jQuery_, _knockout_, _signalR_, and _dotNetify_ from their respective CDNs:
```html
<script src="https://code.jquery.com/jquery-3.1.1.min.js"></script>
<script src="https://cdnjs.cloudflare.com/ajax/libs/knockout/3.4.2/knockout-min.js"></script>
<script src="https://cdn.jsdelivr.net/npm/@aspnet/signalr@1/dist/browser/signalr.min.js"></script>
<script src="https://unpkg.com/dotnetify@3/dist/dotnetify-ko.min.js"></script>
```

The _signalR_ script above is only for ASP.NET Core. If you are using ASP.NET Framework server, replace with:
```html
<script src="https://cdn.jsdelivr.net/npm/signalr@2/jquery.signalR.min.js"></script>
```
