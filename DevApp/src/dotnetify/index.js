import dotnetify from './react/dotnetify-react';
import './react/dotnetify-react.scope';
import './react/dotnetify-react.router';

if (window) {
  window.dotNetify = dotnetify;
  window.dotnetify = dotnetify;
}
module.exports = dotnetify;
