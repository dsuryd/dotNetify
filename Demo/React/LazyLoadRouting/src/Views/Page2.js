import React from 'react';
import { useConnect, RouteLink } from 'dotnetify';
import { lazyLoad } from '../routes';

export default props => {
  const { vm, state } = useConnect('Page2', { props, state: { Title: '', Links: [] } });
  if (vm)
    vm.onRouteEnter = (path, template) => {
      template.Target = 'Page2Content';
      return lazyLoad(template.Id);
    };
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
      <div id="Page2Content" />
    </React.Fragment>
  );
};

export const Page2Home = () => <h3>Item Home</h3>;

export const Page2Item = props => {
  const { state } = useConnect('Page2Item', { props, state: { Title: '' } });
  return (
    <React.Fragment>
      <h3>{state.Title}</h3>
      <p style={{ textAlign: 'justify' }}>
        Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore
        magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo
        consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla
        pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est
        laborum.
      </p>
    </React.Fragment>
  );
};
