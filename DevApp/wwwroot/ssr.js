/*
 This script is used for server-side rendering of dotNetify routing.
 Place this script inside "wwwwroot".
 */
const { JSDOM, ResourceLoader } = require('jsdom');
module.exports = function(callback, userAgent, ...args) {
  JSDOM.fromFile('./wwwroot/index.html', {
    url: 'http://localhost:8080',
    resources: new ResourceLoader({ userAgent }),
    runScripts: 'dangerously',
    beforeParse(window) {
      const document = window.document;
      window.__dotnetify_ssr__ = function(ssr) {
        document.execCommand = function() {};

        const done = function() {
          // Insert styled-components css rules into the head style tag.
          const el = document.querySelector('style[data-styled]');
          if (el) el.innerHTML = Object.values(el.sheet.cssRules).reduce((acc, r) => acc + r.cssText, '');
          callback(null, `<!DOCTYPE html>${document.documentElement.outerHTML}`);
        };

        ssr(done, ...args, 2000 /*timeout*/);
      };
    }
  });
};
