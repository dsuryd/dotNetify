var Index = React.createClass( {

   getInitialState()
   {
      // Connect this component to the back-end view model.
      this.vm = dotnetify.react.connect( "IndexVM", () => this.state, state => this.setState( state ) );

      return {
         Links: []
      }
   },
   render()
   {
      const links = this.state.Links.map( link =>
         <li key={link.Id}>
            <a href={this.vm.$route(link.Route)} onClick={this.vm.$handleRoute}>{link.Caption}</a>
            <div>{link.Description}</div>
         </li>
      );

      return (
         <div>
            <h3>Basic Examples</h3>
            <ul id="BasicExamples">
               {links}
            </ul>
         </div>
      );
   }
} );

ReactDOM.render(
  <Index />,
  document.querySelector( "#NavMenu" )
);
