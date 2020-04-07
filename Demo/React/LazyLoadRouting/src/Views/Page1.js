import React from 'react';
import { useConnect, RouteLink } from 'dotnetify';
import { lazyLoad } from '../routes';

export default props => {
  const { vm, state } = useConnect('Page1', { props, state: { Title: '', Links: [] } });
  if (vm)
    vm.onRouteEnter = (path, template) => {
      template.Target = 'Page1Content';
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
      <div id="Page1Content" />
    </React.Fragment>
  );
};

export const Page1A = () => (
  <div>
    <h3>This is the content of Page 1A</h3>
    <p>
      Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore
      magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo
      consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur.
      Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.
    </p>
  </div>
);

export const Page1B = () => (
  <div>
    <h3>This is the content of Page 1B</h3>
    <p>
      Sed ut perspiciatis unde omnis iste natus error sit voluptatem accusantium doloremque laudantium, totam rem
      aperiam, eaque ipsa quae ab illo inventore veritatis et quasi architecto beatae vitae dicta sunt explicabo. Nemo
      enim ipsam voluptatem quia voluptas sit aspernatur aut odit aut fugit, sed quia consequuntur magni dolores eos qui
      ratione voluptatem sequi nesciunt. Neque porro quisquam est, qui dolorem ipsum quia dolor sit amet, consectetur,
      adipisci velit, sed quia non numquam eius modi tempora incidunt ut labore et dolore magnam aliquam quaerat
      voluptatem. Ut enim ad minima veniam, quis nostrum exercitationem ullam corporis suscipit laboriosam, nisi ut
      aliquid ex ea commodi consequatur? Quis autem vel eum iure reprehenderit qui in ea voluptate velit esse quam nihil
      molestiae consequatur, vel illum qui dolorem eum fugiat quo voluptas nulla pariatur?
    </p>
  </div>
);
