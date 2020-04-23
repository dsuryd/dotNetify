import React from 'react';
import dotnetify, { useConnect, RouteLink, RouteTarget } from 'dotnetify';
import { setRouteTarget } from '../routes';

dotnetify.debug = true;

export default props => {
  const onRouteEnter = setRouteTarget('Content');
  const { vm, state } = useConnect('App', { props, state: { Links: [] } }, { onRouteEnter });

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
      <RouteTarget id="Content" />
    </main>
  );
};
