import React from 'react';
import PropTypes from 'prop-types';

export default class MovieDetails extends React.Component {
  static contextTypes = { connect: PropTypes.func };

  constructor(props, context) {
    super(props, context);
    this.state = { Movie: {} };
    this.context.connect('MovieDetailsVM', this);
  }

  componentWillUnmount() {
    this.vm.$destroy();
  }

  render() {
    const movie = this.state.Movie || {};
    const casts = movie.Cast ? movie.Cast.split(',') : [];

    return (
      <div className="card">
        <header className="card-header">
          <b>{movie.Movie}</b>
          <div>{movie.Year}</div>
        </header>
        <section className="card-body">
          <b>Director</b>
          <p>{movie.Director}</p>
          <b>Cast</b>
          {casts.map((cast, idx) => <div key={idx}>{cast}</div>)}
        </section>
      </div>
    );
  }
}
