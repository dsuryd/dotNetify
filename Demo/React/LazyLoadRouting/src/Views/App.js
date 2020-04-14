import React from 'react';
import dotnetify, { useConnect, RouteLink, RouteTarget } from 'dotnetify';
import { setRouteTarget } from '../routes';

dotnetify.debug = true;

export default props => {
  const onCreated = _vm => setRouteTarget(_vm, 'Content');
  const { vm, state } = useConnect('App', { props, state: { Links: [] } }, onCreated);

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
