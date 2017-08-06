class SecurePage extends React.Component {
   constructor(props) {
      super(props);
      let hasAccessToken = window.sessionStorage.getItem("access_token") != null;
      this.state = { loginError: null, authenticated: hasAccessToken };
   }
   signIn(username, password) {
      var form = new FormData();
      form.append('username', username);
      form.append('password', password);
      fetch('http://localhost:52000/token', { method: 'post', body: form })
         .then(response => {
            if (!response.ok) throw Error(response.statusText);
            return response.json();
         })
         .then(token => {
            window.sessionStorage.setItem("access_token", token.access_token);
            this.setState({ loginError: null, authenticated: true });
         })
         .catch(error => this.setState({ loginError: "Invalid user name or password" }));
   }
   signOut() {
      window.sessionStorage.clear();
      this.setState({ authenticated: false });
   }
   render() {
      let handleSubmit = info => this.signIn(info.username, info.password);
      let handleSignOut = () => this.signOut();
      return (
         <MuiThemeProvider>
            <div className="container-fluid">
               <div className="header clearfix">
                  <h3>Example: Secure Page *** UNDER CONSTRUCTION ***</h3>
               </div>
               <div className="row">
                  <div className="col-md-6">
                     <LoginForm onSubmit={handleSubmit} onSignOut={handleSignOut} errorText={this.state.loginError} authenticated={this.state.authenticated} />
                  </div>
                  <div className="col-md-6">
                     {this.state.authenticated ? <SecurePageView authenticated={true} /> : <div><SecurePageView /></div>}
                  </div>
               </div>
            </div>
         </MuiThemeProvider>
      )
   }
}

class LoginForm extends React.Component {
   constructor(props) {
      super(props);
      this.state = { username: "guest", password: "dotnetify" };
   }
   render() {
      let handleUserNameChange = event => this.setState({ username: event.target.value });
      let handlePasswordChange = event => this.setState({ password: event.target.value });
      let handleSubmit = () => this.props.onSubmit(this.state);
      let handleSignOut = () => this.props.onSignOut();
      return (
         <div className="jumbotron">
            {this.props.authenticated ?
               <div>
                  <h3>Signed in</h3>
                  <RaisedButton label="Sign out" primary={true} onClick={handleSignOut} /> 
               </div> :
               <div>
                  <h3>Sign in</h3>
                  <div>
                     <TextField id="UserName" floatingLabelText="User name" value={this.state.username} onChange={handleUserNameChange} errorText={this.props.errorText} /><br />
                     <TextField id="Password" floatingLabelText="Password" value={this.state.password} onChange={handlePasswordChange} errorText={this.props.errorText} />
                  </div>
                  <RaisedButton label="Submit" primary={true} onClick={handleSubmit} />
               </div>
            }
         </div>
      );
   }
}

class SecurePageView extends React.Component {
   constructor(props) {
      super(props);
      this.state = {SecureCaption: null, SecureData: null};

      let accessToken = window.sessionStorage.getItem("access_token");
      let bearerToken = accessToken ? "Bearer " + accessToken : null;
      let authHeader = bearerToken ? { Authorization: bearerToken } : {};

      this.vm = dotnetify.react.connect("SecurePageVM", this, { headers: authHeader, exceptionHandler: this.onException });
   }
   componentWillUnmount() {
      this.vm.$destroy();
   }
   onException(exception) {
      console.error(exception.message);
   }
   render() {
      return (
         <div className="jumbotron">
            <h3>Secure View</h3>
            <div>{this.state.SecureCaption ? "Authenticated" : "Not authenticated"}</div>
            <h4>{this.state.SecureCaption}</h4>
            <div>{this.state.SecureData}</div>
            <AdminSecurePageView />
         </div>
      );
   }
}

class AdminSecurePageView extends React.Component {
   constructor(props) {
      super(props);
      this.state = { AdminCaption: null };

      let accessToken = window.sessionStorage.getItem("access_token");
      let bearerToken = accessToken ? "Bearer " + accessToken : null;
      let authHeader = bearerToken ? { Authorization: bearerToken } : {};

      this.vm = dotnetify.react.connect("AdminSecurePageVM", this, { headers: authHeader, exceptionHandler: this.onException });
   }
   componentWillUnmount() {
      this.vm.$destroy();
   }
   onException(exception) {
      console.error(exception.message);
   }
   render() {
      const adminCaption = this.state.AdminCaption ? <h3>{this.state.AdminCaption}</h3> : <span />;
      return <div>{adminCaption}</div>;
   }
}

