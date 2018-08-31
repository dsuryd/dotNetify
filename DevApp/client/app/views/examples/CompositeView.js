import React from 'react';
import PropTypes from 'prop-types';
import dotnetify from 'dotnetify';
import styled from 'styled-components';
import RenderExample from '../../components/RenderExample';

const Container = styled.div`
  padding: 0 1rem;
  display: flex;
  > * {
    &:first-child {
      margin-right: 1rem;
    }
    &:last-child {
      width: 20%;
      min-width: 10rem;
    }
  }
  .card {
    p {
      font-size: unset;
    }
    .card-header > *:first-child {
      font-size: large;
    }
  }
  table {
    font-size: unset;
    tr {
      &:hover {
        background: #eee;
        cursor: pointer;
      }
      &.selected {
        background: #ddd;
      }
    }
    td,
    th {
      padding: 0.5rem 0;
      padding-right: 2rem;
      border-bottom: 1px solid #ddd;
    }
  }
  .pagination {
    user-select: none;
    div {
      margin-top: 1rem;
      padding: .5rem 1rem;
      border: 1px solid #ccc;
      &.current {
        color: #aaa;
        background: #eee;
      }
    }
    div:hover:not(.current) {
      background: #eee;
      cursor: pointer;
    }
  }
`;

const Scope = dotnetify.react.Scope;

class CompositeView extends React.Component {
  constructor(props) {
    super(props);
    this.state = { FirstName: '', LastName: '' };

    // Set up function to dispatch state to the back-end.
    this.dispatchState = state => this.vm.$dispatch(state);
  }

  componentWillUnmount() {
    this.vm.$destroy();
  }

  render() {
    return (
      <Container>
        <Scope vm="CompositeViewVM">
          <Scope vm="FilterableMovieTableVM">
            <MovieTable />
          </Scope>
          <aside>
            <MovieDetails />
          </aside>
        </Scope>
      </Container>
    );
  }
}

class MovieTable extends React.Component {
  static contextTypes = { connect: PropTypes.func };

  constructor(props, context) {
    super(props, context);
    this.state = this.context.connect('MovieTableVM', this) || { Headers: [], Data: [], Pagination: [] };
  }

  componentWillUnmount() {
    this.vm.$destroy();
  }

  render() {
    return (
      <section>
        <table>
          <tbody>
            <tr>{this.state.Headers.map((text, idx) => <th key={idx}>{text}</th>)}</tr>
            {this.state.Data.map((data, idx) => (
              <tr
                key={idx}
                className={this.state.SelectedKey === data.Rank && 'selected'}
                onClick={_ => this.dispatchState({ SelectedKey: data.Rank })}
              >
                <td>{data.Rank}</td>
                <td>{data.Movie}</td>
                <td>{data.Year}</td>
                <td>{data.Cast}</td>
                <td>{data.Director}</td>
              </tr>
            ))}
          </tbody>
        </table>
        <div className="pagination">
          {this.state.Pagination.map(num => (
            <div
              key={num}
              className={this.state.SelectedPage === num && 'current'}
              onClick={_ => this.dispatchState({ SelectedPage: num })}
            >
              {num}
            </div>
          ))}
        </div>
      </section>
    );
  }
}

class MovieDetails extends React.Component {
  static contextTypes = { connect: PropTypes.func };

  constructor(props, context) {
    super(props, context);
    this.state = this.context.connect('MovieDetailsVM', this) || { Movie: {} };
  }

  componentWillUnmount() {
    this.vm.$destroy();
  }

  render() {
    const movie = this.state.Movie || {};
    const casts = movie.Cast ? movie.Cast.split(',') : [];

    return (
      <div class="card">
        <header class="card-header">
          <b>{movie.Movie}</b>
          <div>{movie.Year}</div>
        </header>
        <section class="card-body">
          <p>
            <b>Director</b>
            <div>{movie.Director}</div>
          </p>
          <p>
            <b>Cast</b>
            {casts.map((cast, idx) => <div key={idx}> {cast} </div>)}
          </p>
        </section>
      </div>
    );
  }
}

export default _ => (
  <RenderExample vm="CompositeViewExample">
    <CompositeView />
  </RenderExample>
);
