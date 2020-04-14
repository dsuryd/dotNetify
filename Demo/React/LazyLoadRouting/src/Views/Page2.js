import React from 'react';
import { useConnect, RouteLink, RouteTarget } from 'dotnetify';
import { setRouteTarget } from '../routes';

export default props => {
  const onCreated = _vm => setRouteTarget(_vm, 'Page2Content');
  const { vm, state } = useConnect('Page2', { props, state: { Title: '', Links: [] } }, onCreated);

  return (
    <React.Fragment>
      <h1>{state.Title}</h1>
      <ul>
        {state.Links.map((link, idx) => (
          <li key={idx}>
            <RouteLink vm={vm} route={link.Route}>
              {link.Title}
            </RouteLink>
          </li>
        ))}
      </ul>
      <RouteTarget id="Page2Content" />
    </React.Fragment>
  );
};

export const Page2Home = () => <h3>Item Home</h3>;

export const Page2Item = props => {
  const { state } = useConnect('Page2Item', { props, state: { Title: '' } });
  return (
    <React.Fragment>
      <h3>{state.Title}</h3>
      <p style={{ textAlign: 'justify' }}>{state.Content} </p>
    </React.Fragment>
  );
};
