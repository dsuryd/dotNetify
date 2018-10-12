import React from 'react';
import Vue from 'vue';

export default class RenderVue extends React.Component {
  constructor(props) {
    super(props);
    this.setRoutingArgs();
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

  setRoutingArgs() {
    const { htmlAttrs } = this.props;
    if (htmlAttrs) {
      for (let key in htmlAttrs) {
        debugger;
        //if (key === 'vmRoot') $(vmElem).attr('data-vm-root', htmlAttrs[key]);
        //else if (key === 'vmArg') $(vmElem).attr('data-vm-arg', JSON.stringify(htmlAttrs[key]));
      }
    }
  }
}
