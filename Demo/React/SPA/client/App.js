import React from 'react';
import { useConnect, RouteLink } from 'dotnetify';

export default () => {
  const { vm, state } = useConnect('App', { Links: [] });
  if (vm) vm.onRouteEnter = (path, template) => (template.Target = 'Content');

  return (
    <div>
      <nav className="navbar navbar-expand-lg navbar-light bg-light">
        {state.Links.map(link => (
          <RouteLink key={link.Id} vm={vm} route={link.Route} style={{ paddingRight: '2rem' }}>
            {link.Caption}
          </RouteLink>
        ))}
      </nav>
      <div id="Content" style={{ paddingTop: '1rem' }} />
    </div>
  );
};
