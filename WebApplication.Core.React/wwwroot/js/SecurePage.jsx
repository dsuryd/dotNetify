var SecurePage = React.createClass({
   getInitialState() {
      this.vm = dotnetify.react.connect("SecurePageVM", this);
      return {Title: ""};
   },
   componentWillUnmount() {
      this.vm.$destroy();
   },
   render() {
      return (
         <MuiThemeProvider>
            <div className="container-fluid">
               <div className="header clearfix">
                  <h3>Example: Secure Page *** UNDER CONSTRUCTION ***</h3>
               </div>
               <div>{this.state.Title}</div>
            </div>
         </MuiThemeProvider>
      )
   }
});