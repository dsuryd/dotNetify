'use strict';

var _createClass = (function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ('value' in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; })();

var _get = function get(_x, _x2, _x3) { var _again = true; _function: while (_again) { var object = _x, property = _x2, receiver = _x3; _again = false; if (object === null) object = Function.prototype; var desc = Object.getOwnPropertyDescriptor(object, property); if (desc === undefined) { var parent = Object.getPrototypeOf(object); if (parent === null) { return undefined; } else { _x = parent; _x2 = property; _x3 = receiver; _again = true; desc = parent = undefined; continue _function; } } else if ('value' in desc) { return desc.value; } else { var getter = desc.get; if (getter === undefined) { return undefined; } return getter.call(receiver); } } };

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError('Cannot call a class as a function'); } }

function _inherits(subClass, superClass) { if (typeof superClass !== 'function' && superClass !== null) { throw new TypeError('Super expression must either be null or a function, not ' + typeof superClass); } subClass.prototype = Object.create(superClass && superClass.prototype, { constructor: { value: subClass, enumerable: false, writable: true, configurable: true } }); if (superClass) Object.setPrototypeOf ? Object.setPrototypeOf(subClass, superClass) : subClass.__proto__ = superClass; }

var SecurePage = (function (_React$Component) {
   _inherits(SecurePage, _React$Component);

   function SecurePage(props) {
      _classCallCheck(this, SecurePage);

      _get(Object.getPrototypeOf(SecurePage.prototype), 'constructor', this).call(this, props);
      var hasAccessToken = window.sessionStorage.getItem("access_token") != null;
      this.state = { loginError: null, authenticated: hasAccessToken };
   }

   _createClass(SecurePage, [{
      key: 'signIn',
      value: function signIn(username, password) {
         var _this = this;

         var form = new FormData();
         form.append('username', username);
         form.append('password', password);
         fetch('http://localhost:52000/token', { method: 'post', body: form }).then(function (response) {
            if (!response.ok) throw Error(response.statusText);
            return response.json();
         }).then(function (token) {
            window.sessionStorage.setItem("access_token", token.access_token);
            _this.setState({ loginError: null, authenticated: true });
         })['catch'](function (error) {
            return _this.setState({ loginError: "Invalid user name or password" });
         });
      }
   }, {
      key: 'signOut',
      value: function signOut() {
         window.sessionStorage.clear();
         this.setState({ authenticated: false });
      }
   }, {
      key: 'render',
      value: function render() {
         var _this2 = this;

         var handleSubmit = function handleSubmit(info) {
            return _this2.signIn(info.username, info.password);
         };
         var handleSignOut = function handleSignOut() {
            return _this2.signOut();
         };
         return React.createElement(
            MuiThemeProvider,
            null,
            React.createElement(
               'div',
               { className: 'container-fluid' },
               React.createElement(
                  'div',
                  { className: 'header clearfix' },
                  React.createElement(
                     'h3',
                     null,
                     'Example: Secure Page *** UNDER CONSTRUCTION ***'
                  )
               ),
               this.state.authenticated ? React.createElement(SecurePageView, { onSignOut: handleSignOut }) : React.createElement(LoginForm, { onSubmit: handleSubmit, errorText: this.state.loginError })
            )
         );
      }
   }]);

   return SecurePage;
})(React.Component);

var LoginForm = (function (_React$Component2) {
   _inherits(LoginForm, _React$Component2);

   function LoginForm(props) {
      _classCallCheck(this, LoginForm);

      _get(Object.getPrototypeOf(LoginForm.prototype), 'constructor', this).call(this, props);
      this.state = { username: "demo", password: "dotnetify" };
   }

   _createClass(LoginForm, [{
      key: 'render',
      value: function render() {
         var _this3 = this;

         var handleUserNameChange = function handleUserNameChange(event) {
            return _this3.setState({ username: event.target.value });
         };
         var handlePasswordChange = function handlePasswordChange(event) {
            return _this3.setState({ password: event.target.value });
         };
         var handleSubmit = function handleSubmit() {
            return _this3.props.onSubmit(_this3.state);
         };
         return React.createElement(
            'div',
            { className: 'jumbotron' },
            React.createElement(
               'h3',
               null,
               'Sign in'
            ),
            React.createElement(
               'p',
               null,
               React.createElement(TextField, { id: 'UserName', floatingLabelText: 'User name', value: this.state.username, onChange: handleUserNameChange, errorText: this.props.errorText }),
               React.createElement('br', null),
               React.createElement(TextField, { id: 'Password', floatingLabelText: 'Password', value: this.state.password, onChange: handlePasswordChange, errorText: this.props.errorText })
            ),
            React.createElement(RaisedButton, { label: 'Submit', primary: true, onClick: handleSubmit })
         );
      }
   }]);

   return LoginForm;
})(React.Component);

var SecurePageView = (function (_React$Component3) {
   _inherits(SecurePageView, _React$Component3);

   function SecurePageView(props) {
      _classCallCheck(this, SecurePageView);

      _get(Object.getPrototypeOf(SecurePageView.prototype), 'constructor', this).call(this, props);
      this.state = {};

      var bearerToken = "Bearer " + window.sessionStorage.getItem("access_token");
      this.vm = dotnetify.react.connect("SecurePageVM", this, { headers: { Authorization: bearerToken } });
   }

   _createClass(SecurePageView, [{
      key: 'componentWillUnmount',
      value: function componentWillUnmount() {
         this.vm.$destroy();
      }
   }, {
      key: 'render',
      value: function render() {
         var _this4 = this;

         var handleSignOut = function handleSignOut() {
            return _this4.props.onSignOut();
         };
         return React.createElement(
            'div',
            { className: 'jumbotron' },
            React.createElement(
               'h3',
               null,
               'Secure View'
            ),
            React.createElement(RaisedButton, { label: 'Sign out', primary: true, onClick: handleSignOut })
         );
      }
   }]);

   return SecurePageView;
})(React.Component);

