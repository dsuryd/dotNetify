import React from 'react';
import dotnetify from 'dotnetify';

export default class HelloWorld extends React.Component {
  constructor(props) {
    super(props);
    this.state = { Greetings: '', ServerTime: '' };

    dotnetify.react.connect('HelloWorld', this);
  }

  render() {
    return (
      <div>
        <p>{this.state.Greetings}</p>
        <p>Server time is: {this.state.ServerTime}</p>
      </div>
    );
  }
}
