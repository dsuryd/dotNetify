import React from 'react';
import PropTypes from 'prop-types';

export default class MovieTable extends React.Component {
  static contextTypes = { connect: PropTypes.func };

  constructor(props, context) {
    super(props, context);
    this.state = { Headers: [], Data: [], Pagination: [] };
    this.context.connect('MovieTableVM', this);
  }

  componentWillUnmount() {
    this.vm.$destroy();
  }

  render() {
    return (
      <div>
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
      </div>
    );
  }
}
