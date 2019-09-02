import dotnetify, { Scope, RouteLink, useConnect } from './react';

export { Scope, RouteLink, useConnect };
export default dotnetify;

if (window) {
  window.dotNetify = dotnetify;
  window.dotnetify = dotnetify;
}
