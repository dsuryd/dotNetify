##### SecurePage.js

```jsx
import React from 'react';
import dotnetify from 'dotnetify';
import { SecurePageCss } from './components/css';
import TextBox from './components/TextBox';
import 'whatwg-fetch';

class SecurePage extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      loginError: null,
      accessToken: window.sessionStorage.getItem('access_token')
    };
  }

  signIn(username, password) {
    fetch('/token', {
      method: 'post',
      mode: 'no-cors',
      body: 'username=' + username + '&password=' + password + '&grant_type=password&client_id=dotnetifydemo',
      headers: { 'Content-Type': 'application/x-www-form-urlencoded;charset=UTF-8' }
    })
      .then(response => {
        if (!response.ok) throw Error(response.statusText);
        return response.json();
      })
      .then(token => {
        window.sessionStorage.setItem('access_token', token.access_token);
        this.setState({ loginError: null, accessToken: token.access_token });
      })
      .catch(error => this.setState({ loginError: 'Invalid user name or password' }));
  }

  signOut() {
    window.sessionStorage.clear();
    this.setState({ accessToken: null });
  }

  render() {
    return (
      <SecurePageCss>
        <LoginForm
          onSubmit={info => this.signIn(info.username, info.password)}
          onSignOut={() => this.signOut()}
          errorText={this.state.loginError}
          authenticated={this.state.accessToken != null}
        />
        {this.state.accessToken ? (
          <SecurePageView accessToken={this.state.accessToken} onExpiredAccess={() => this.signOut()} />
        ) : (
          <NotAuthenticatedView />
        )}
      </SecurePageCss>
    );
  }
}

const NotAuthenticatedView = () => (
  <div className="card">
    <div className="card-body">
      <h4>Not authenticated</h4>
    </div>
  </div>
);
```

##### LoginForm.js

```jsx
class LoginForm extends React.Component {
  state = { username: '', password: 'dotnetify' };

  render() {
    if (this.props.authenticated)
      return (
        <div className="card logout">
          <button className="btn btn-primary" onClick={_ => this.props.onSignOut()}>
            Sign Out
          </button>
        </div>
      );

    return (
      <div className="card">
        <div className="card-header">
          <h4>Sign in</h4>
        </div>
        <div className="card-body">
          <TextBox
            label="User name:"
            placeholder="Type guest or admin"
            value={this.state.username}
            onChange={value => this.setState({ username: value })}
            errorText={this.props.errorText}
          />
          <TextBox
            label="Password:"
            type="password"
            value={this.state.password}
            onChange={value => this.setState({ password: value })}
            errorText={this.props.errorText}
          />
        </div>
        <div className="card-footer">
          <button className="btn btn-primary" onClick={_ => this.props.onSubmit(this.state)}>
            Submit
          </button>
        </div>
      </div>
    );
  }
}
```

##### SecurePageView.js

```jsx
class SecurePageView extends React.Component {
  constructor(props) {
    super(props);
    this.state = { SecureCaption: 'Not authenticated' };

    if (this.props.accessToken) {
      let authHeader = { Authorization: 'Bearer ' + this.props.accessToken };
      this.vm = dotnetify.react.connect('SecurePageVM', this, {
        headers: authHeader,
        exceptionHandler: ex => this.onException(ex)
      });
    }
  }

  componentWillUnmount() {
    this.vm && this.vm.$destroy();
  }

  onException(exception) {
    if (exception.name == 'UnauthorizedAccessException') this.props.onExpiredAccess && this.props.onExpiredAccess();
  }

  render() {
    let handleExpiredAccess = () => this.props.onExpiredAccess();
    return (
      <div className="card">
        <div className="card-header">
          <h4>{this.state.SecureCaption}</h4>
        </div>
        <div className="card-body">
          <strong>{this.state.SecureData}</strong>
          <AdminSecurePageView accessToken={this.props.accessToken} onExpiredAccess={handleExpiredAccess} />
        </div>
      </div>
    );
  }
}
```

##### AdminSecurePageView.js

```jsx
class AdminSecurePageView extends React.Component {
  constructor(props) {
    super(props);
    this.state = {};

    if (this.props.accessToken) {
      let authHeader = { Authorization: 'Bearer ' + this.props.accessToken };
      this.vm = dotnetify.react.connect('AdminSecurePageVM', this, { headers: authHeader, exceptionHandler: ex => {} });
    }
  }

  componentWillUnmount() {
    this.vm && this.vm.$destroy();
  }

  render() {
    if (!this.state.TokenIssuer) return null;
    return (
      <div>
        <h5>Admin-only view:</h5>
        <div>{this.state.TokenIssuer}</div>
        <div>{this.state.TokenValidFrom}</div>
        <div>{this.state.TokenValidTo}</div>
      </div>
    );
  }
}
```

##### SecurePageVM.cs

```csharp
[Authorize]
[SetAccessToken]
public class SecurePageVM : BaseVM
{
  private Timer _timer;
  private string _userName;
  private SecurityToken _accessToken;
  private int AccessExpireTime => (int)(_accessToken.ValidTo - DateTime.UtcNow).TotalSeconds;

  public string SecureCaption { get; set; }
  public string SecureData { get; set; }

  public SecurePageVM(IPrincipalAccessor principalAccessor)
  {
      _userName = principalAccessor.Principal?.Identity.Name;
  }

  public override void Dispose() => _timer?.Dispose();

  public void SetAccessToken(SecurityToken accessToken)
  {
      _accessToken = accessToken;
      SecureCaption = $"Authenticated user: \"{_userName}\"";
      Changed(nameof(SecureCaption));

      _timer = _timer ?? new Timer(state =>
      {
        SecureData = _accessToken != null ? $"Access token will expire in {AccessExpireTime} seconds" : null;
        Changed(nameof(SecureData));
        PushUpdates();
      }, null, 0, 1000);
  }
}
```

##### AdminSecurePageVM.cs

```csharp
[Authorize(Role = "admin")]
[SetAccessToken]
public class AdminSecurePageVM : BaseVM
{
  public string TokenIssuer { get; set; }
  public string TokenValidFrom { get; set; }
  public string TokenValidTo { get; set; }

  public void SetAccessToken(SecurityToken accessToken)
  {
      TokenIssuer = $"Token issuer: \"{accessToken.Issuer}\"";
      TokenValidFrom = $"Valid from: {accessToken.ValidFrom.ToString("R")}";
      TokenValidTo = $"Valid to: {accessToken.ValidTo.ToString("R")}";

      Changed(nameof(TokenIssuer));
      Changed(nameof(TokenValidFrom));
      Changed(nameof(TokenValidTo));
  }
}
```