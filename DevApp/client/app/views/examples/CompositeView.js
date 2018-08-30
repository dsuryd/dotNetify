import React from 'react';
import PropTypes from 'prop-types';
import dotnetify from 'dotnetify';
import styled from 'styled-components';
import RenderExample from '../../components/RenderExample';

const Container = styled.div`
  padding: 0 1rem;
  table {
    font-size: unset;
    tr:hover {
      background: #eee;
      cursor: pointer;
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
          <FilterableMovieTableVM />
        </Scope>
      </Container>
    );
  }
}

class FilterableMovieTableVM extends React.Component {
  static contextTypes = { connect: PropTypes.func };

  render() {
    return (
      <Scope vm="FilterableMovieTableVM">
        <MovieTable />
      </Scope>
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
              <tr key={idx}>
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

export default _ => (
  <RenderExample vm="CompositeViewExample">
    <CompositeView />
  </RenderExample>
);
