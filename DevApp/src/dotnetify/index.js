import dotnetify, { Scope, RouteLink } from './react';

export { Scope, RouteLink };
export default dotnetify;

if (window) {
  window.dotNetify = dotnetify;
  window.dotnetify = dotnetify;
}
