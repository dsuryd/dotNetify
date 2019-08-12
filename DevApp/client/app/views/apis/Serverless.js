import React from 'react';
import { Markdown, withTheme } from 'dotnetify-elements';
import { NavMenu, VMContext } from 'dotnetify-elements';
import Article from '../../components/Article';
import Expander from '../../components/Expander';
import { currentFramework, frameworkSelectEvent } from 'app/components/SelectFramework';

class Serverless extends React.Component {
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
    return framework === 'Vue' ? <ServerlessVue /> : <ServerlessReact />;
  }
}

const ServerlessReact = _ => (
  <Article vm="Serverless" id="Content">
    <Markdown id="Content">
      <Markdown>{serverlessHelloWorldReactCode()}</Markdown>
      <Expander label={<SeeItLive />} content={<ServerlessHelloWorld />} />
      <Expander label={<SeeItLive />} content={<ServerlessNav />} />
    </Markdown>
  </Article>
);

const ServerlessVue = _ => (
  <Article vm="ServerlessVue" id="Content">
    <Markdown id="Content">
      <Markdown>{serverlessHelloWorldVueCode()}</Markdown>
      <Expander label={<SeeItLive />} content={<ServerlessHelloWorldVue />} />
    </Markdown>
  </Article>
);

const SeeItLive = _ => <b>See It Live!</b>;

const serverlessHelloWorldReactCode = () =>
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

class ServerlessHelloWorld extends React.Component {
  constructor(props) {
    super(props);
    this.vm = dotnetify.react.connect('ServerlessHelloWorld', this, localHelloWorld);
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

const serverlessHelloWorldVueCode = () =>
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

class ServerlessHelloWorldVue extends React.Component {
  componentDidMount() {
    this.app = new Vue(
      dotnetify.vue.component(
        {
          name: 'ServerlessHelloWorld',
          template: `
          <div>
            <div>{{state.Greetings}}</div>
            <input type="text" v-model="state.Name" />
          </div>
        `
        },
        'ServerlessHelloWorld',
        { watch: [ 'Name' ], ...localHelloWorldVue }
      )
    );
    this.app.$mount('#ServerlessHelloWorldVue');
  }
  componentWillUnmount() {
    this.app.$destroy();
  }
  render() {
    return <div id="ServerlessHelloWorldVue" />;
  }
}

const localNav = {
  mode: 'local',
  initialState: {
    RoutingState: {},
    NavMenu: [
      {
        Route: {
          TemplateId: null,
          Path: 'core/overview',
          RedirectRoot: ''
        },
        Label: 'Introduction'
      },
      {
        IsExpanded: true,
        Routes: [
          {
            Route: {
              TemplateId: null,
              Path: 'core/examples/helloworld',
              RedirectRoot: ''
            },
            Label: 'Hello World',
            Icon: 'material-icons web'
          },
          {
            Route: {
              TemplateId: null,
              Path: 'core/examples/livechart',
              RedirectRoot: ''
            },
            Label: 'Live Chart',
            Icon: 'material-icons show_chart'
          }
        ],
        Label: 'Examples'
      }
    ]
  }
};

const ServerlessNav = _ => (
  <VMContext vm="localNav" options={localNav}>
    <NavMenu id="NavMenu" />
  </VMContext>
);

export default withTheme(Serverless);
