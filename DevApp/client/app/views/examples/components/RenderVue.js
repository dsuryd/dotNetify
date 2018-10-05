import React from 'react';
import Vue from 'vue';
import dotnetify from 'dotnetify';

export default class RenderVue extends React.Component {
  constructor(props) {
    super(props);
  }

  componentDidMount() {
    new Vue({
      el: '.vue',
      components: {
        'vue-root': this.props.src
      }
    });
  }

  render() {
    return <section className="vue" dangerouslySetInnerHTML={{ __html: '<vue-root />' }} />;
  }
}
