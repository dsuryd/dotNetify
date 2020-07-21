## Get Started

The easiest way to get started is to perform the following step-by-step instructions given in the _Overview_ to create a new project from scratch with either Vue CLI, WebPack or script tag.  
[inset]

#### Client Setup

If you use module bundler like WebPack, install **dotnetify** and `vue` from NPM:

```jsx
npm i --save dotnetify vue
```

If using script tags, include _vue_, _signalR_, and _dotNetify_ from their respective CDNs:

```html
<script src="https://cdn.jsdelivr.net/npm/vue"></script>
<script src="https://cdn.jsdelivr.net/npm/@aspnet/signalr@1/dist/browser/signalr.min.js"></script>
<script src="https://unpkg.com/dotnetify@3/dist/dotnetify-vue.min.js"></script>
```

### Typescript Support

DotNetify's typed definition for React can be used in Vue projects with minimal steps:

- Add React's type definition to _devDependencies_: `npm i -D @types/react`.
- Include `"skipLibCheck": true` to _tsconfig.json_.
