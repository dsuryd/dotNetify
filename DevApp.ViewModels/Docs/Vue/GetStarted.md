## Get Started

The easiest way to get started is to perform the following step-by-step instructions given in the _Overview_ to create a new project from scratch with either Vue CLI, WebPack or script tag.  
[inset]

#### Client-Side Library

If you use module bundler like WebPack, install __dotnetify__ and `vue` from NPM: 
```jsx
npm i --save dotnetify
npm i --save vue
```

The library uses SignalR .NET Core client by default. To switch to .NET Framework client, add:
```jsx
import signalR from 'dotnetify/dist/signalR-netfx';
dotnetify.hubLib = signalR;
```


If using script tags, include _vue_, _signalR_, and _dotNetify_ from their respective CDNs:
```html
<script src="https://cdn.jsdelivr.net/npm/vue"></script>
<script src="https://cdn.jsdelivr.net/npm/@aspnet/signalr@1/dist/browser/signalr.min.js"></script>
<script src="https://unpkg.com/dotnetify@3/dist/dotnetify-vue.min.js"></script>
```

To use SignalR .NET Framework client instead of .NET Core, replace _signalR_ script above with:
```html
<script src="https://code.jquery.com/jquery-3.1.1.min.js"></script>
<script src="https://cdn.jsdelivr.net/npm/signalr@2/jquery.signalR.min.js"></script>
```
