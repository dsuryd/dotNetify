import dotnetify from './dotnetify-react';
import './dotnetify-react.router';
import enableSsr from './dotnetify-react.router.ssr';
import Scope from './Scope';
import RouteLink from './RouteLink';
import RouteTarget from './RouteTarget';
import useConnect from './useConnect';

module.exports = Object.assign(dotnetify, { Scope, RouteLink, RouteTarget, enableSsr, useConnect });
