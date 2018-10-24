import React from 'react';
import dotnetify from 'dotnetify';

export default class RenderVue extends React.Component {
	componentDidMount() {
		dotnetify.vue.router.$mount('.example-root.vue', this.props.src, this.props.htmlAttrs);
	}

	render() {
		return <section className="example-root vue" />;
	}
}
