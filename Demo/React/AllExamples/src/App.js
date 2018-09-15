import React from 'react';
import dotnetify from 'dotnetify';

export default class App extends React.Component {
  constructor(props) {
    super(props);
    dotnetify.react.connect('App', this);
  }

  render() {
    return <div />;
  }
}
