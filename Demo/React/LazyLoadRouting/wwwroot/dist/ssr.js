/*
 This script is used for server-side rendering of dotNetify routing.
 Place this script inside "wwwwroot".
 */
const { JSDOM } = require('jsdom');
module.exports = function(callback, requestPath, vmInitialState) {
  JSDOM.fromFile('./wwwroot/index.html', {
    url: 'file://wwwroot/',
    resources: 'usable',
    runScripts: 'dangerously',
    beforeParse(window) {
      window.__dotnetify_ssr__ = function(ssr) {
        ssr(callback, requestPath, vmInitialState, 2000 /*timeout*/);
      };
    }
  });
};
