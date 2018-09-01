import React from 'react';
import PropTypes from 'prop-types';
import dotnetify from 'dotnetify';
import styled from 'styled-components';
import TextBox from './components/TextBox';
import RenderExample from '../../components/RenderExample';

const Container = styled.div`
  padding: 0 1rem;
  max-width: 1268px;
  display: flex;
  flex-wrap: wrap;
  > * {
    &:first-child {
      width: 70%;
      margin-right: 1rem;
    }
    &:last-child {
      width: calc(30% - 1rem);
      min-width: 10rem;
    }
  }
  @media (max-width: 860px) {
    > * {
      width: 100%;
      &:first-child {
        width: 100%;
        margin-bottom: 2rem;
      }
      &:last-child {
        width: 100%;
      }
    }
  }
  .card {
    width: 100%;
    p {
      font-size: unset;
    }
    .card-header > *:first-child {
      font-size: large;
    }
  }
  table {
    font-size: unset;
    width: 100%;
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
      width: 25%;
      padding: .5rem;
      padding-right: 2rem;
      border-bottom: 1px solid #ddd;
      &:nth-child(1),
      &:nth-child(3) {
        width: 10%;
      }
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
  .filter {
    margin-top: 1rem;
    .card-header {
      font-weight: bold;
    }
    .card-body {
      > * {
        margin-bottom: 1rem;
      }
    }
    .card-footer {
      display: flex;
      justify-content: flex-end;
    }
    .chip {
      margin-right: .25rem;
      margin-bottom: .25rem;
      display: inline-flex;
      font-size: x-small;
      background: #eee;
      border-radius: .25rem;
      align-items: center;
      padding: .2rem .4rem;
      i.material-icons {
        font-size: small;
        font-weight: bold;
        cursor: pointer;
        margin-left: .5rem;
      }
    }
  }
`;

const Scope = dotnetify.react.Scope;

const CompositeView = () => (
  <Container>
    <Scope vm="CompositeViewVM">
      <Scope vm="FilterableMovieTableVM">
        <MovieTable />
      </Scope>
      <aside>
        <MovieDetails />
        <Scope vm="FilterableMovieTableVM">
          <MovieFilter />
        </Scope>
      </aside>
    </Scope>
  </Container>
);

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
                className={this.state.SelectedKey === data.Rank ? 'selected' : ''}
                onClick={_ => this.dispatchState({ SelectedKey: data.Rank })}
              >
                <td>{data.Rank}</td>
                <td>{data.Movie}</td>
                <td>{data.Year}</td>
                <td>{data.Director}</td>
              </tr>
            ))}
          </tbody>
        </table>
        <div className="pagination">
          {this.state.Pagination.map(num => (
            <div
              key={num}
              className={this.state.SelectedPage === num ? 'current' : ''}
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

class MovieFilter extends React.Component {
  static contextTypes = { connect: PropTypes.func };

  constructor(props, context) {
    super(props, context);
    // Combine state from back-end with local state.
    // This can be more concise using Object.assign if not for IE 11 support.
    var state = this.context.connect('MovieFilterVM', this) || {};
    var localState = {
      filters: [],
      filterId: 0,
      filter: 'Any',
      operation: 'contains',
      operations: [ 'contains' ],
      text: ''
    };
    for (var prop in localState) state[prop] = localState[prop];
    this.state = state;
  }

  componentWillUnmount() {
    this.vm.$destroy();
  }

  handleApply = () => {
    let newId = this.state.filterId + 1;
    let filter = {
      id: newId,
      property: this.state.filter,
      operation: this.state.operation,
      text: this.state.text
    };
    this.setState({
      filterId: newId,
      filters: [ filter, ...this.state.filters ],
      text: ''
    });
    this.dispatch({ Apply: filter });

    this.updateFilterDropdown('Any'); // Reset filter dropdown.
  };

  handleDelete = id => {
    this.dispatch({ Delete: id });
    this.setState({ filters: this.state.filters.filter(filter => filter.id != id) });
  };

  updateFilterDropdown = value => {
    this.setState({ filter: value });
    if (value == 'Rank' || value == 'Year')
      this.setState({ filter: value, operations: [ 'equals', '>=', '<=' ], operation: 'equals' });
    else this.setState({ filter: value, operations: [ 'contains' ], operation: 'contains' });
  };

  render() {
    const movieProps = [ 'Any', 'Rank', 'Movie', 'Year', 'Cast', 'Director' ];

    const filters = this.state.filters.map(filter => (
      <div className="chip" key={filter.id}>
        {filter.property} {filter.operation} {filter.text}
        <div onClick={() => this.handleDelete(filter.id)}>
          <i className="material-icons">clear</i>
        </div>
      </div>
    ));

    return (
      <form>
        <div className="filter card">
          <div className="card-header">Filters</div>
          <div className="card-body">
            <select
              className="form-control"
              value={this.state.filter}
              onChange={e => this.updateFilterDropdown(e.target.value)}
            >
              {movieProps.map((text, idx) => (
                <option key={idx} value={text}>
                  {text}
                </option>
              ))}
            </select>
            <div className="operation ">
              {this.state.operations.map((text, idx) => (
                <div key={idx} className="form-check">
                  <input
                    id={text}
                    className="form-check-input"
                    type="radio"
                    value={text}
                    checked={this.state.operation === text}
                    onChange={_ => this.setState({ operation: text })}
                  />
                  <label className="form-check-label" htmlFor={text}>
                    {text}
                  </label>
                </div>
              ))}
            </div>
            <TextBox id="FilterText" value={this.state.text} onChange={val => this.setState({ text: val })} />
            <div>{filters}</div>
          </div>
          <div className="card-footer">
            <button className="btn btn-primary" onClick={this.handleApply} disabled={!this.state.text}>
              Apply
            </button>
          </div>
        </div>
      </form>
    );
  }
}

export default _ => (
  <RenderExample vm="CompositeViewExample">
    <CompositeView />
  </RenderExample>
);
