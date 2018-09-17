"use strict";

var _createClass = (function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; })();

var _get = function get(_x, _x2, _x3) { var _again = true; _function: while (_again) { var object = _x, property = _x2, receiver = _x3; _again = false; if (object === null) object = Function.prototype; var desc = Object.getOwnPropertyDescriptor(object, property); if (desc === undefined) { var parent = Object.getPrototypeOf(object); if (parent === null) { return undefined; } else { _x = parent; _x2 = property; _x3 = receiver; _again = true; desc = parent = undefined; continue _function; } } else if ("value" in desc) { return desc.value; } else { var getter = desc.get; if (getter === undefined) { return undefined; } return getter.call(receiver); } } };

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

function _inherits(subClass, superClass) { if (typeof superClass !== "function" && superClass !== null) { throw new TypeError("Super expression must either be null or a function, not " + typeof superClass); } subClass.prototype = Object.create(superClass && superClass.prototype, { constructor: { value: subClass, enumerable: false, writable: true, configurable: true } }); if (superClass) Object.setPrototypeOf ? Object.setPrototypeOf(subClass, superClass) : subClass.__proto__ = superClass; }

dotnetify.debug = true;

var RouteLink = dotnetify.react.router.RouteLink;

var Index = (function (_React$Component) {
   _inherits(Index, _React$Component);

   function Index(props) {
      _classCallCheck(this, Index);

      _get(Object.getPrototypeOf(Index.prototype), "constructor", this).call(this, props);
      // Connect this component to the back-end view model.
      this.vm = dotnetify.react.connect("IndexVM", this);
      this.vm.onRouteEnter = function (path, template) {
         return template.Target = "Content";
      };

      this.state = window.vmStates.IndexVM || {};
      this.state["selectedLink"] = "";
   }

   _createClass(Index, [{
      key: "render",
      value: function render() {
         var _this = this;

         var styles = {
            navMenu: { padding: "15px", color: "rgb(125,135,140)", backgroundColor: "rgb(40,50,55)" },
            link: { color: "#b8c7ce" },
            exampleLink: { color: "#b8c7ce", display: "block", padding: "5px 0px 5px 20px", textDecoration: "none" },
            activeExampleLink: { color: "black", backgroundColor: "#eef0f0", display: "block", padding: "5px 0px 5px 20px", textDecoration: "none" },
            desc: { padding: "0 15px 0 35px", fontSize: "9pt", marginBottom: ".5em" },
            header: { fontSize: "medium", color: "rgba(146,208,80,.8)" },
            list: { paddingLeft: "0", listStyleType: "none", margin: "0 -15px" },
            listItem: { paddingLeft: "20px" },
            bullet: { color: "rgba(255,205,0,.8)", transform: "scale(.5)" },
            copyright: { color: "rgb(125,135,140)", fontSize: "8pt" }
         };

         var setSelectedLink = function setSelectedLink(linkId) {
            return _this.setState({ selectedLink: linkId });
         };

         var showLinks = function showLinks(links) {
            return links.map(function (link) {
               return React.createElement(
                  "li",
                  { key: link.Id },
                  React.createElement(
                     RouteLink,
                     { vm: _this.vm, route: link.Route,
                        style: link.Id == _this.state.selectedLink ? styles.activeExampleLink : styles.exampleLink,
                        className: "example-link",
                        onClick: function () {
                           return setSelectedLink(link.Id);
                        } },
                     React.createElement("span", { style: styles.bullet, className: "glyphicon glyphicon-asterisk" }),
                     link.Caption
                  ),
                  React.createElement(
                     "div",
                     { style: styles.desc },
                     link.Description
                  )
               );
            });
         };

         return React.createElement(
            "div",
            { style: styles.navMenu },
            React.createElement(
               "a",
               { style: styles.link, href: "http://dotnetify.net" },
               "Website"
            ),
            React.createElement(
               "div",
               null,
               React.createElement(
                  "h3",
                  { style: styles.header },
                  "Basic Examples"
               ),
               React.createElement(
                  "ul",
                  { id: "BasicExamples", style: styles.list },
                  showLinks(this.state.BasicExampleLinks)
               ),
               React.createElement(
                  "h3",
                  { style: styles.header },
                  "Further Examples"
               ),
               React.createElement(
                  "ul",
                  { id: "FurtherExamples", style: styles.list },
                  showLinks(this.state.FurtherExampleLinks)
               )
            ),
            React.createElement(
               "div",
               { style: styles.copyright },
               React.createElement("br", null),
               React.createElement(
                  "small",
                  null,
                  "© 2015-2017 Dicky Suryadi.  All code licensed under the ",
                  React.createElement(
                     "a",
                     { style: styles.link, href: "http://www.apache.org/licenses/LICENSE-2.0" },
                     "Apache license version 2.0"
                  )
               ),
               React.createElement("br", null),
               React.createElement("br", null)
            )
         );
      }
   }]);

   return Index;
})(React.Component);

ReactDOM.render(React.createElement(Index, null), document.querySelector("#NavMenu"));

