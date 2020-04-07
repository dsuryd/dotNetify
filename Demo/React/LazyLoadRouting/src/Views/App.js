import React from 'react';
import dotnetify, { useConnect, RouteLink } from 'dotnetify';
import { lazyLoad } from '../routes';

dotnetify.debug = true;

export default props => {
  const { vm, state } = useConnect('App', { props, state: { Links: [] } });
  if (vm)
    vm.onRouteEnter = (path, template) => {
      template.Target = 'Content';
      return lazyLoad(template.Id);
    };
  return (
    <main>
      <ul>
        {state.Links.map((link, idx) => (
          <li key={idx}>
            <RouteLink vm={vm} route={link.Route}>
              {link.Title}
            </RouteLink>
          </li>
        ))}
      </ul>
      <section id="Content" />
    </main>
  );
};
