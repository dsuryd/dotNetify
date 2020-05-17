import dotnetify, { Scope, RouteLink, useConnect } from "./react";

export { Scope, RouteLink, useConnect };
export default dotnetify;

if (window) Object.assign(window, { dotnetify, dotNetify: dotnetify });
