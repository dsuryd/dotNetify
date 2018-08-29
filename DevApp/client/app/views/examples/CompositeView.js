import React from 'react';
import PropTypes from 'prop-types';
import dotnetify from 'dotnetify';
import styled from 'styled-components';
import RenderExample from '../../components/RenderExample';

const Container = styled.div`
  padding: 0 1rem;
  table {
    font-size: unset;
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
    this.state = this.context.connect('MovieTableVM', this) || { Headers: [], Data: [] };
  }
  componentWillUnmount() {
    this.vm.$destroy();
  }
  render() {
    return (
      <table>
        <tbody>
          <tr>{this.state.Headers.map((text, idx) => <th key={idx}>{text}</th>)}</tr>
          {this.state.Data.map((data, idx) => (
            <tr>
              <td key={idx}>{data.Rank}</td>
              <td key={idx}>{data.Movie}</td>
              <td key={idx}>{data.Year}</td>
              <td key={idx}>{data.Cast}</td>
              <td key={idx}>{data.Director}</td>
            </tr>
          ))}
        </tbody>
      </table>
    );
  }
}

export default _ => (
  <RenderExample vm="CompositeViewExample">
    <CompositeView />
  </RenderExample>
);
