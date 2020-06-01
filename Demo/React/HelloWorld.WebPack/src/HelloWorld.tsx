import React from 'react';
import dotnetify from 'dotnetify';

interface State {
  Greetings: string;
  ServerTime: string;
}

export class HelloWorld extends React.Component<any, State> {
  state: State = { Greetings: '', ServerTime: '' };

  constructor(props: any) {
    super(props);
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
