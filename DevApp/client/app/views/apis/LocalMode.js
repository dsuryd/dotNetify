import React from 'react';
import { Markdown, withTheme } from 'dotnetify-elements';
import { Alert, Frame, NavMenu, Panel, VMContext } from 'dotnetify-elements';
import Article from '../../components/Article';
import Expander from '../../components/Expander';
import { currentFramework, frameworkSelectEvent } from 'app/components/SelectFramework';

class LocalMode extends React.Component {
  constructor() {
    super();
    this.state = { framework: currentFramework };
    this.unsubs = frameworkSelectEvent.subscribe(framework => this.setState({ framework: framework }));
  }
  componentWillUnmount() {
    this.unsubs();
  }
  render() {
    const { framework } = this.state;
    return framework === 'Vue' ? <LocalModeVue /> : <LocalModeReact />;
  }
}

const LocalModeReact = _ => (
  <Article vm="LocalMode" id="Content">
    <Markdown id="Content">
      <Markdown>{localModeHelloWorldReactCode()}</Markdown>
      <Expander label={<SeeItLive />} content={<LocalModeHelloWorld />} />
      <Expander label={<SeeItLive />} content={<LocalModeApp />} />
    </Markdown>
  </Article>
);

const LocalModeVue = _ => (
  <Article vm="LocalModeVue" id="Content">
    <Markdown id="Content">
      <Markdown>{localModeHelloWorldVueCode()}</Markdown>
      <Expander label={<SeeItLive />} content={<LocalModeHelloWorldVue />} />
    </Markdown>
  </Article>
);

const SeeItLive = _ => <b>See It Live!</b>;

const localModeHelloWorldReactCode = () =>
  `
\`\`\`jsx
const mockHelloWorld = {
  mode: 'local',
  initialState: { Greetings: 'Hello World' },
  onDispatch: function(data) {
    this.update({ Greetings: 'Hello ' + data.Submit.Name });
  }
};

class MyApp extends React.Component {
  constructor(props) {
    super(props);
    this.vm = dotnetify.react.connect('HelloWorld', this, mockHelloWorld);
    this.state = { Greetings: '', name: '' };
  }
  render() {
    const handleName = e => this.setState({ name: e.target.value });
    const handleSubmit = () => this.vm.$dispatch({ Submit: { Name: this.state.name } });
    return (
      <div>
        <div>{this.state.Greetings}</div>
        <input type="text" value={this.state.Name} onChange={handleName} />
        <button onClick={handleSubmit}>Submit</button>
      </div>
    );
  }
}
\`\`\``;

const localHelloWorld = {
  mode: 'local',
  initialState: { Greetings: 'Hello World' },
  onDispatch: function(data) {
    this.update({ Greetings: 'Hello ' + data.Submit.Name });
  }
};

class LocalModeHelloWorld extends React.Component {
  constructor(props) {
    super(props);
    this.vm = dotnetify.react.connect('LocalModeHelloWorld', this, localHelloWorld);
    this.state = { Greetings: '', name: '' };
  }
  componentWillUnmount() {
    this.vm.$destroy();
  }
  render() {
    const handleName = e => this.setState({ name: e.target.value });
    const handleSubmit = () => this.vm.$dispatch({ Submit: { Name: this.state.name } });
    return (
      <div>
        <div>{this.state.Greetings}</div>
        <input type="text" value={this.state.Name} onChange={handleName} />
        <button onClick={handleSubmit}>Submit</button>
      </div>
    );
  }
}

const localModeHelloWorldVueCode = () =>
  `
\`\`\`jsx
<template>
  <div>
    <div>{{state.Greetings}}</div>
    <input type="text" v-model="state.Name" />
  </div>
</template>
<script>
  import dotnetify from 'dotnetify/vue';

  const localHelloWorld = {
    mode: 'local',
    initialState: { Greetings: 'Hello World' },
    onDispatch: function(data) {
      this.update({ Greetings: 'Hello ' + data.Name });
    }
  };

  export default dotnetify.vue.component('MyApp', 'HelloWorld', { 
    watch: [ 'Name' ], 
    ...localHelloWorld 
  });
</script>

\`\`\``;

const localHelloWorldVue = {
  mode: 'local',
  initialState: { Greetings: 'Hello World' },
  onDispatch: function(data) {
    this.update({ Greetings: 'Hello ' + data.Name });
  }
};

class LocalModeHelloWorldVue extends React.Component {
  componentDidMount() {
    this.app = new Vue(
      dotnetify.vue.component(
        {
          name: 'LocalModeHelloWorld',
          template: `
          <div>
            <div>{{state.Greetings}}</div>
            <input type="text" v-model="state.Name" />
          </div>
        `
        },
        'LocalModeHelloWorld',
        { watch: [ 'Name' ], ...localHelloWorldVue }
      )
    );
    this.app.$mount('#LocalModeHelloWorldVue');
  }
  componentWillUnmount() {
    this.app.$destroy();
  }
  render() {
    return <div id="LocalModeHelloWorldVue" />;
  }
}
window.LocalPage1 = _ => <Alert>You selected Page 1</Alert>;
window.LocalPage2 = _ => <Alert danger>You selected Page 2</Alert>;

window.LocalMode_LocalVM = {
  initialState: {
    RoutingState: {
      Templates: [
        {
          Id: 'Page1',
          Root: '',
          UrlPattern: 'page1',
          ViewUrl: 'LocalPage1'
        },
        {
          Id: 'Page2',
          Root: '',
          UrlPattern: 'page2',
          ViewUrl: 'LocalPage2'
        }
      ],
      Root: 'core/api/localmode'
    },
    NavMenu: [
      {
        Route: {
          TemplateId: 'Page1',
          Path: 'page1'
        },
        Label: 'Page 1'
      },
      {
        Route: {
          TemplateId: 'Page2',
          Path: 'page2'
        },
        Label: 'Page 2'
      }
    ]
  }
};

const LocalModeApp = _ => (
  <VMContext vm="LocalVM" options={{ mode: 'local' }}>
    <Panel horizontal>
      <Panel flex="30%">
        <NavMenu id="NavMenu" target="localTarget" />
      </Panel>
      <Panel flex="70%" css="padding-top: .5rem">
        <div id="localTarget" />
      </Panel>
    </Panel>
  </VMContext>
);

export default withTheme(LocalMode);
