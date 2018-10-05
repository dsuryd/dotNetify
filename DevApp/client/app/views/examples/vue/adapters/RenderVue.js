import React from 'react';
import Vue from 'vue';

export default class RenderVue extends React.Component {
	constructor(props) {
		super(props);
	}

	componentDidMount() {
		new Vue({
			el: '.vue',
			components: {
				'vue-example': this.props.src
			}
		});
	}

	render() {
		return <section className="vue" dangerouslySetInnerHTML={{ __html: '<vue-example />' }} />;
	}
}
