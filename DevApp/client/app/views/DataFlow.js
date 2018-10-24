import React from 'react';
import Vue from 'vue';
import dotnetify from 'dotnetify';
import { Markdown, withTheme } from 'dotnetify-elements';
import styled from 'styled-components';
import Article from '../components/Article';
import Expander from '../components/Expander';
import SingleVMImage from '../images/SingleVMPattern.svg';
import SiloedVMImage from '../images/SiloedVMPattern.svg';
import ScopedVMImage from '../images/ScopedVMPattern.svg';
import { currentFramework, frameworkSelectEvent } from 'app/components/SelectFramework';

const Image = styled.img`
  display: flex;
  margin: 2rem auto;
  align-items: center;
  justify-content: center;
`;

class DataFlow extends React.Component {
  constructor() {
    super();
    this.state = { framework: currentFramework };
    this.unsubs = frameworkSelectEvent.subscribe(framework => this.setState({ framework: framework }));
  }
  componentWillUnmount() {
    this.unsubs();
  }
  componentDidUpdate() {
    if (this.state.framework === 'Knockout') dotnetify.react.router.pushState({}, null, '/core/overview');
  }
  render() {
    const { framework } = this.state;
    return framework === 'React' ? <DataFlowReact /> : framework === 'Vue' ? <DataFlowVue /> : null;
  }
}

const DataFlowReact = props => (
  <Article vm="DataFlow" id="Content">
    <Markdown id="Content">
      <Image src={SingleVMImage} width="350px" />
      <Image src={SiloedVMImage} width="350px" />
      <Image src={ScopedVMImage} width="400px" />
      <Expander label={<SeeItLive />} content={<MasterDetails />} />
    </Markdown>
  </Article>
);

const DataFlowVue = props => (
  <Article vm="DataFlowVue" id="Content">
    <Markdown id="Content">
      <Image src={SingleVMImage} width="350px" />
      <Image src={SiloedVMImage} width="350px" />
      <Image src={ScopedVMImage} width="400px" />
      <Expander label={<SeeItLive />} content={<MasterDetailsVue />} />
    </Markdown>
  </Article>
);

const SeeItLive = _ => <b>See It Live!</b>;

const MasterDetails = _ => (
  <div style={{ display: 'flex' }}>
    <MasterList />
    <Details />
  </div>
);

class MasterList extends React.Component {
  constructor() {
    super();
    this.state = { ListItems: [], selected: 0 };
    this.vm = dotnetify.react.connect('MasterDetails.MasterList', this);
  }

  componentWillUnmount() {
    this.vm.$destroy();
  }

  handleSelect = id => {
    this.setState({ selected: id });
    this.vm.$dispatch({ Select: id });
  };

  render() {
    const itemStyle = selected => ({ cursor: 'pointer', background: selected ? '#eee' : 'none' });
    const items = this.state.ListItems.map(item => (
      <li key={item.Id} style={itemStyle(item.Id === this.state.selected)} onClick={() => this.handleSelect(item.Id)}>
        {item.Title}
      </li>
    ));
    return <ul>{items}</ul>;
  }
}

class Details extends React.Component {
  constructor() {
    super();
    this.state = { ItemImageUrl: null };
    this.vm = dotnetify.react.connect('MasterDetails.Details', this);
  }

  componentWillUnmount() {
    this.vm.$destroy();
  }

  render() {
    if (!this.state.ItemImageUrl) return null;
    return <img src={this.state.ItemImageUrl} style={{ margin: '0 1rem' }} />;
  }
}

class MasterDetailsVue extends React.Component {
  componentDidMount() {
    this.app = new Vue({
      el: '#MasterDetailsVue',
      components: {
        MasterList: MasterListVue,
        Details: DetailsVue
      },
      template: `
      <div style='display: flex'>
          <MasterList />
          <Details />
        </div>
      `
    });
  }
  componentWillUnmount() {
    this.app.$destroy();
  }
  render() {
    return <div id="MasterDetailsVue" />;
  }
}

const MasterListVue = Vue.component('MasterList', {
  template: `
    <ul>
      <li 
        v-for="item in ListItems" 
        :key="item.Id" 
        :style="getStyle(item.Id)" 
        @click="onClick(item.Id)"
      >
        {{item.Title}}
      </li>  
    </ul>
  `,
  created() {
    this.vm = dotnetify.vue.connect('MasterDetails.MasterList', this);
  },
  destroyed() {
    this.vm.$destroy();
  },
  data() {
    return { ListItems: [], selected: null };
  },
  methods: {
    getStyle(id) {
      return { cursor: 'pointer', background: id === this.selected ? '#eee' : 'none' };
    },
    onClick(id) {
      this.selected = id;
      this.vm.$dispatch({ Select: id });
    }
  }
});

const DetailsVue = Vue.component('Details', {
  template: '<img v-if="ItemImageUrl" :src="ItemImageUrl" style="margin: 0 1rem" />',
  created() {
    this.vm = dotnetify.vue.connect('MasterDetails.Details', this);
  },
  destroyed() {
    this.vm.$destroy();
  },
  data() {
    return { ItemImageUrl: null };
  }
});

export default withTheme(DataFlow);
