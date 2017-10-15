class SecurePage extends React.Component {
   constructor(props) {
      super(props);
      this.state = { loginError: null, accessToken: window.sessionStorage.getItem("access_token") };
   }
   signIn(username, password) {
      var form = new FormData();
      form.append('username', username);
      form.append('password', password); // Demo only; don't submit clear text pwd in prod code!
      fetch('http://localhost:52000/token', { method: 'post', body: form })
         .then(response => {
            if (!response.ok) throw Error(response.statusText);
            return response.json();
         })
         .then(token => {
            window.sessionStorage.setItem("access_token", token.access_token);
            this.setState({ loginError: null, accessToken: token.access_token });
         })
         .catch(error => this.setState({ loginError: "Invalid user name or password" }));
   }
   signOut() {
      window.sessionStorage.clear();
      this.setState({ accessToken: null });
   }
   render() {
      const handleSubmit = info => this.signIn(info.username, info.password);
      const handleSignOut = () => this.signOut();
      const handleExpiredAccess = () => this.signOut();
      return (
         <MuiThemeProvider>
            <div className="container-fluid">
               <div className="header clearfix">
                  <h3>Example: Secure Page</h3>
               </div>
               <div>
                  <LoginForm
                     onSubmit={handleSubmit}
                     onSignOut={handleSignOut}
                     errorText={this.state.loginError}
                     authenticated={this.state.accessToken != null} />
                  <br />
                  {this.state.accessToken ? <SecurePageView accessToken={this.state.accessToken} onExpiredAccess={handleExpiredAccess} /> : <NotAuthenticatedView />}
               </div>
            </div>
         </MuiThemeProvider>
      )
   }
}

class LoginForm extends React.Component {
   constructor(props) {
      super(props);
      this.state = { username: "", password: "dotnetify" };
   }
   render() {
      const handleUserNameChange = event => this.setState({ username: event.target.value });
      const handlePasswordChange = event => this.setState({ password: event.target.value });
      const handleSubmit = () => this.props.onSubmit(this.state);
      const handleSignOut = () => this.props.onSignOut();
      return (
         <Paper zDepth={1} style={{ padding: "2em", backgroundColor: '#f6f6f6' }}>
            {this.props.authenticated ?
               <div>
                  <RaisedButton label="Sign out" secondary={true} onClick={handleSignOut} />
               </div> :
               <div>
                  <h3>Sign in</h3>
                  <div>
                     <TextField id="UserName" floatingLabelText="User name" value={this.state.username} onChange={handleUserNameChange} errorText={this.props.errorText} floatingLabelFixed={true} hintText="Type guest or admin" /><br />
                     <TextField id="Password" floatingLabelText="Password" value={this.state.password} onChange={handlePasswordChange} errorText={this.props.errorText} />
                  </div>
                  <RaisedButton label="Submit" primary={true} onClick={handleSubmit} />
               </div>
            }
         </Paper>
      );
   }
}

const NotAuthenticatedView = () => 
   <Paper style={{ padding: "2em" }}>
      <h4>Not authenticated</h4>
   </Paper>

class SecurePageView extends React.Component {
   constructor(props) {
      super(props);
      this.state = { SecureCaption: "Not authenticated" };

      if (this.props.accessToken) {
         let authHeader = { Authorization: "Bearer " + this.props.accessToken };
         this.vm = dotnetify.react.connect("SecurePageVM", this, { headers: authHeader, exceptionHandler: ex => this.onException(ex) });
      }
   }
   componentWillUnmount() {
      this.vm && this.vm.$destroy();
   }
   onException(exception) {
      if (exception.name == "UnauthorizedAccessException")
         this.props.onExpiredAccess && this.props.onExpiredAccess();
   }
   render() {
      let handleExpiredAccess = () => this.props.onExpiredAccess();
      return (
         <Paper style={{ padding: "2em" }}>
            <h4>{this.state.SecureCaption}</h4>
            <div>{this.state.SecureData}</div>
            <AdminSecurePageView accessToken={this.props.accessToken} onExpiredAccess={handleExpiredAccess} />
         </Paper>
      );
   }
}

class AdminSecurePageView extends React.Component {
   constructor(props) {
      super(props);
      this.state = {};

      if (this.props.accessToken) {
         let authHeader = { Authorization: "Bearer " + this.props.accessToken };
         this.vm = dotnetify.react.connect("AdminSecurePageVM", this, { headers: authHeader, exceptionHandler: ex => { } });
      }
   }
   componentWillUnmount() {
      this.vm && this.vm.$destroy();
   }
   render() {
      return !this.state.TokenIssuer ? null :
         <div>
            <br />
            <h4>Admin-only view:</h4>
            <div>
               {this.state.TokenIssuer}<br />
               {this.state.TokenValidFrom}<br />
               {this.state.TokenValidTo}
            </div>
         </div>;
   }
}

