import $ from 'jquery';
import React from 'react';
import dotnetify from 'dotnetify';

export default class RenderKnockout extends React.Component {
  constructor(props) {
    super(props);
    this.koRef = React.createRef();
  }

  componentDidMount() {
    this.setRoutingArgs();
    dotnetify.ko.init();
  }

  componentWillUnmount() {
    dotnetify.ko.destroy();
  }

  render() {
    return <section ref={this.koRef} className="example-root knockout" dangerouslySetInnerHTML={{ __html: this.props.html }} />;
  }

  setRoutingArgs() {
    const { htmlAttrs } = this.props;
    const vmElem = $(this.koRef.current).find('[data-vm]')[0];
    if (htmlAttrs && vmElem) {
      for (let key in htmlAttrs) {
        if (key === 'vmRoot') $(vmElem).attr('data-vm-root', htmlAttrs[key]);
        else if (key === 'vmArg') $(vmElem).attr('data-vm-arg', JSON.stringify(htmlAttrs[key]));
      }
    }
  }
}
