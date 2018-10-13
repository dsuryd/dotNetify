import React from 'react';
import Vue from 'vue';

export default class RenderVue extends React.Component {
  constructor(props) {
    super(props);
    this.template = `<${props.src.name} ${this.setRoutingArgs()} />`;
    this.componentName = props.src.name.toLowerCase();
  }

  componentDidMount() {
    this.vue = new Vue({
      el: '.vue',
      components: {
        [this.componentName]: this.props.src
      }
    });
  }
  componentWillUnmount() {
    this.vue.$destroy();
  }

  render() {
    return <section className="example-root vue" dangerouslySetInnerHTML={{ __html: this.template }} />;
  }

  setRoutingArgs() {
    const { htmlAttrs } = this.props;
    let props = '';
    if (htmlAttrs) {
      for (let key in htmlAttrs) {
        if (key === 'vmRoot') props += `vm-root='${htmlAttrs[key]}'`;
        else if (key === 'vmArg') props += ` vm-arg='${JSON.stringify(htmlAttrs[key])}'`;
      }
    }
    return props;
  }
}
