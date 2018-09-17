import React from 'react';

const redirect = path => window.dotnetify.react.router.pushState({}, null, path);

const FromScratchLink = _ => (
  <ul style={{ fontSize: '1.1rem' }}>
    <li>
      <a href="javascript:void(0)" onClick={_ => redirect('/core/fromscratch-scripttag')}>
        Real-time "Hello World" from scratch with .NET Core CLI + Script Tag
      </a>
    </li>
    <li>
      <a href="javascript:void(0)" onClick={_ => redirect('/core/fromscratch-webpack')}>
        Real-time "Hello World" from scratch with Visual Studio + WebPack
      </a>
    </li>
  </ul>
);

export default FromScratchLink;
