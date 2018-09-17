dotnetify.debug = true;

const RouteLink = dotnetify.react.router.RouteLink;

class Index extends React.Component {
   constructor(props) {
      super(props);
      // Connect this component to the back-end view model.
      this.vm = dotnetify.react.connect("IndexVM", this);
      this.vm.onRouteEnter = (path, template) => template.Target = "Content";

      this.state = window.vmStates.IndexVM || {};
      this.state["selectedLink"] = "";
   }
   render() {
      const styles = {
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
      }

      const setSelectedLink = (linkId) =>  this.setState({selectedLink: linkId});

      const showLinks = links => links.map(link =>
         <li key={link.Id}>
            <RouteLink vm={this.vm} route={link.Route}
                       style={link.Id == this.state.selectedLink ? styles.activeExampleLink : styles.exampleLink} 
                       className="example-link"
                       onClick={() => setSelectedLink(link.Id)}>
               <span style={styles.bullet} className="glyphicon glyphicon-asterisk"></span>
               {link.Caption}
            </RouteLink>
            <div style={styles.desc}>{link.Description}</div>
         </li>
      );

      return (
         <div style={styles.navMenu}>
            <a style={styles.link} href="http://dotnetify.net">Website</a>
            <div>
               <h3 style={styles.header}>Basic Examples</h3>
               <ul id="BasicExamples" style={styles.list}>
                  {showLinks(this.state.BasicExampleLinks)}
               </ul>
               <h3 style={styles.header}>Further Examples</h3>
               <ul id="FurtherExamples" style={styles.list}>
                  {showLinks(this.state.FurtherExampleLinks)}
               </ul>
            </div>

            <div style={styles.copyright}>
               <br />
               <small>
                  © 2015-2017 Dicky Suryadi.&nbsp;
                  All code licensed under the <a style={styles.link} href="http://www.apache.org/licenses/LICENSE-2.0">Apache license version 2.0</a>
               </small>
               <br /><br />
            </div>
         </div>
      );
   }
}

ReactDOM.render(
  <Index />,
  document.querySelector("#NavMenu")
);
