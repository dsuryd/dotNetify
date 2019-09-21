import React from 'react';
import { Scope } from 'dotnetify';
import { CompositeViewCss } from '../components/css';
import MovieTable from './MovieTable';
import MovieDetails from './MovieDetails';

const CompositeView = () => (
  <CompositeViewCss>
    <Scope vm="CompositeViewVM">
      <MovieTable />
      <aside>
        <MovieDetails />
      </aside>
    </Scope>
  </CompositeViewCss>
);

export default CompositeView;
