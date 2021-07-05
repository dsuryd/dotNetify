##### SecurePage.js

```jsx
import React, { useState } from "react";
import { useConnect } from "dotnetify";
import "whatwg-fetch";
import TextBox from "../components/TextBox";
import { SecurePageCss } from "../components/css";

async function fetchToken(username, password) {
  const response = await fetch("/token", {
    method: "post",
    body: "username=" + username + "&password=" + password + "&grant_type=password&client_id=dotnetifydemo",
    headers: {
      "Content-Type": "application/x-www-form-urlencoded;charset=UTF-8"
    }
  });
  if (!response.ok) throw Error(response.statusText);
  return await response.json();
}

export const SecurePage = () => {
  const [loginError, setLoginError] = useState();
  const [accessToken, setAccessToken] = useState(window.sessionStorage.getItem("access_token"));

  const signIn = (username, password) => {
    fetchToken(username, password)
      .then(token => {
        window.sessionStorage.setItem("access_token", token.access_token);
        setLoginError(null);
        setAccessToken(token.access_token);
      })
      .catch(_ => setLoginError("Invalid user name or password"));
  };

  const signOut = () => {
    window.sessionStorage.clear();
    setAccessToken(null);
  };

  return (
    <SecurePageCss>
      <LoginForm
        onSubmit={info => signIn(info.username, info.password)}
        onSignOut={() => signOut()}
        errorText={loginError}
        authenticated={accessToken != null}
      />
      {accessToken ? <SecurePageView accessToken={accessToken} onExpiredAccess={() => signOut()} /> : <NotAuthenticatedView />}
    </SecurePageCss>
  );
};

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
const LoginForm = ({ authenticated, errorText, onSubmit, onSignOut }) => {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("dotnetify");

  if (authenticated)
    return (
      <div className="card logout">
        <button className="btn btn-primary" onClick={_ => onSignOut()}>
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
          value={username}
          onChange={value => setUsername(value)}
          errorText={errorText}
        />
        <TextBox label="Password:" type="password" value={password} onChange={value => setPassword(value)} errorText={errorText} />
      </div>
      <div className="card-footer">
        <button className="btn btn-primary" onClick={_ => onSubmit({ username, password })}>
          Submit
        </button>
      </div>
    </div>
  );
};
```

##### SecurePageView.js

```jsx
const SecurePageView = ({ accessToken, onExpiredAccess }) => {
  const { state } = useConnect(
    "SecurePageVM",
    { SecureCaption: "Not authenticated" },
    {
      headers: { Authorization: "Bearer " + accessToken },
      exceptionHandler: ex => {}
    }
  );

  const handleExpiredAccess = () => this.props.onExpiredAccess();
  return (
    <div className="card">
      <div className="card-header">
        <h4>{state.SecureCaption}</h4>
      </div>
      <div className="card-body">
        <p>
          <b>{state.SecureData}</b>
        </p>
        <AdminSecurePageView accessToken={accessToken} onExpiredAccess={handleExpiredAccess} />
      </div>
    </div>
  );
};
```

##### AdminSecurePageView.js

```jsx
const AdminSecurePageView = ({ accessToken }) => {
  const { vm, state } = useConnect(
    "AdminSecurePageVM",
    {},
    {
      headers: { Authorization: "Bearer " + accessToken },
      exceptionHandler: ex => {}
    }
  );

  const handleRefreshToken = () => {
    fetchToken("admin", "dotnetify").then(token =>
      vm.$dispatch({ $headers: { Authorization: "Bearer " + token.access_token }, Refresh: true })
    );
  };

  if (!state.TokenIssuer) return null;
  return (
    <section>
      <h5>Admin-only view:</h5>
      <div>{state.TokenIssuer}</div>
      <div>{state.TokenValidFrom}</div>
      <div>{state.TokenValidTo}</div>
      <br />
      <button className="btn btn-secondary" onClick={handleRefreshToken}>
        Refresh Token
      </button>
    </section>
  );
};
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
