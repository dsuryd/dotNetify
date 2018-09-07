import React from 'react';
import dotnetify from 'dotnetify';

export default class RenderKnockout extends React.Component {
  componentDidMount() {
    dotnetify.ko.init();
  }
  render() {
    return <section dangerouslySetInnerHTML={{ __html: this.props.html }} />;
  }
}
