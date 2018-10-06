import React from 'react';
import Vue from 'vue';

export default class RenderVue extends React.Component {
  constructor(props) {
    super(props);
  }

  componentDidMount() {
    this.vue = new Vue({
      el: '.vue',
      components: {
        'vue-example': this.props.src
      }
    });
  }
  componentWillUnmount() {
    this.vue.$destroy();
  }

  render() {
    return <section className="example-root vue" dangerouslySetInnerHTML={{ __html: '<vue-example />' }} />;
  }
}
