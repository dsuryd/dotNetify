import React from 'react';
import dotnetify, { RouteLink } from 'dotnetify';

export default class App extends React.Component {
  constructor(props) {
    super(props);
    this.state = { Links: [] };

    this.vm = dotnetify.react.connect('App', this);
    this.vm.onRouteEnter = (path, template) => (template.Target = 'Content');
  }

  componentWillUnmount() {
    this.vm.$destroy();
  }

  render() {
    return (
      <div>
        <nav className="navbar navbar-expand-lg navbar-light bg-light">
          {this.state.Links.map(link => (
            <RouteLink key={link.Id} vm={this.vm} route={link.Route} style={{ paddingRight: '2rem' }}>
              {link.Caption}
            </RouteLink>
          ))}
        </nav>
        <div id="Content" />
      </div>
    );
  }
}
