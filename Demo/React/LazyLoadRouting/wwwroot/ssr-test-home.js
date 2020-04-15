// Run: node ./wwwroot/ssr-test-home
const ssr = require('./ssr.js');

ssr(
  function(_, output) {
    var body = output.match(/<body[^>]*>[\s\S]*<\/body>/gi);
    console.log(body[0]);
  },
  '/',
  ` {"App":{"RoutingState":{"Templates":[{"Id":"Home","Root":null,"UrlPattern":"","ViewUrl":null,"JSModuleUrl":null,"Target":null},{"Id":"Page1","Root":null,"UrlPattern":"Page1","ViewUrl":null,"JSModuleUrl":null,"Target":null},{"Id":"Page2","Root":null,"UrlPattern":"Page2","ViewUrl":null,"JSModuleUrl":null,"Target":null}],"Root":"","Active":null,"Origin":""},"Links":[{"Title":"Home","Route":{"TemplateId":"Home","Path":"","RedirectRoot":null}},{"Title":"Page 1","Route":{"TemplateId":"Page1","Path":"Page1","RedirectRoot":null}},{"Title":"Page 2","Route":{"TemplateId":"Page2","Path":"Page2","RedirectRoot":null}}]},"Home":{"Greetings":"Hello World!","ServerTime":"2020-04-13T19:56:38.6519003-07:00"}}`
);
