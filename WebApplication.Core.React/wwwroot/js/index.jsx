var Index = React.createClass({

   getInitialState() {
      // Connect this component to the back-end view model.
      this.vm = dotnetify.react.connect("IndexVM", () => this.state, state => this.setState(state));
      return window.vmStates.IndexVM || { BasicExampleLinks: [], FurtherExampleLinks: []};
   },
   render() {
      const showLinks = links => links.map(link =>
         <li key={link.Id}>
            <a href={this.vm.$route(link.Route)} onClick={this.vm.$handleRoute}>
               <span className="section-bullet glyphicon glyphicon-asterisk"></span>
               {link.Caption}
            </a>
            <div>{link.Description}</div>
         </li>
      );

      return (
         <div>
            <h3>Basic Examples</h3>
            <ul id="BasicExamples">
               {showLinks(this.state.BasicExampleLinks)}
            </ul>
            <h3>Further Examples</h3>
            <ul id="FurtherExamples">
               {showLinks(this.state.FurtherExampleLinks)}
            </ul>
         </div>
      );
   }
});

ReactDOM.render(
  <Index />,
  document.querySelector("#NavMenu")
);
