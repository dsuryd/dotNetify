/*
 This script is used for server-side rendering of dotNetify routing.
 Place this script inside "wwwwroot".
 */
const { JSDOM, ResourceLoader } = require('jsdom');
module.exports = function(callback, userAgent, ...args) {
  JSDOM.fromFile('./wwwroot/index.html', {
    url: 'http://localhost:5000',
    resources: new ResourceLoader({ userAgent }),
    runScripts: 'dangerously',
    beforeParse(window) {
      window.__dotnetify_ssr__ = function(ssr) {
        ssr(callback, ...args, 2000 /*timeout*/);
      };
    }
  });
};
