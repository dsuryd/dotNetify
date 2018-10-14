import React from 'react';
import dotnetify from 'dotnetify';

export default class RenderVue extends React.Component {
  componentDidMount() {
    const self = this;
    dotnetify.vue.router.$mount('.example-root.vue', this.props.src, this.props.htmlAttrs).then(vue => (self.vue = vue));
  }
  componentWillUnmount() {
    this.vue.$destroy();
  }

  render() {
    return <section className="example-root vue" />;
  }
}
