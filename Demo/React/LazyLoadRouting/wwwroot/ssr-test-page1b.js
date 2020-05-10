// Run: node ./wwwroot/ssr-test-page1b
const ssr = require('./ssr.js');

ssr(
  function(_, output) {
    var body = output.match(/<body[^>]*>[\s\S]*<\/body>/gi);
    console.log(body[0]);
  },
  'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.122 Safari/537.36',
  '/Page1/Page1B',
  `{"App":{"RoutingState":{"Origin":"","Templates":[{"Id":"Home","Root":null,"UrlPattern":"","ViewUrl":null,"JSModuleUrl":null,"Target":null},{"Id":"Page1","Root":null,"UrlPattern":"Page1","ViewUrl":null,"JSModuleUrl":null,"Target":null},{"Id":"Page2","Root":null,"UrlPattern":"Page2","ViewUrl":null,"JSModuleUrl":null,"Target":null}],"Root":"","Active":null},"Links":[{"Title":"Home","Route":{"TemplateId":"Home","Path":"","RedirectRoot":null}},{"Title":"Page1","Route":{"TemplateId":"Page1","Path":"Page1","RedirectRoot":null}},{"Title":"Page2","Route":{"TemplateId":"Page2","Path":"Page2","RedirectRoot":null}}]},"Page1":{"RoutingState":{"Origin":"Page1","Templates":[{"Id":"Page1Home","Root":null,"UrlPattern":"","ViewUrl":"Page1A","JSModuleUrl":null,"Target":null},{"Id":"Page1A","Root":null,"UrlPattern":"Page1A","ViewUrl":null,"JSModuleUrl":null,"Target":null},{"Id":"Page1B","Root":null,"UrlPattern":"Page1B","ViewUrl":null,"JSModuleUrl":null,"Target":null}],"Root":"Page1","Active":null},"Title":"Page1","Links":[{"Title":"Page1A","Route":{"TemplateId":"Page1A","Path":"Page1A","RedirectRoot":null}},{"Title":"Page1B","Route":{"TemplateId":"Page1B","Path":"Page1B","RedirectRoot":null}}]}}`
);
