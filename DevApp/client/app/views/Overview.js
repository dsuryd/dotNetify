import React, { useState } from 'react';
import Vue from 'vue';
import dotnetify, { useConnect } from 'dotnetify';
import { Markdown, Tab, TabItem, withTheme } from 'dotnetify-elements';
import Article from '../components/Article';
import Expander from '../components/Expander';
import FromScratchLink from './from-scratch/FromScratchLink';
import { currentFramework, frameworkSelectEvent } from 'app/components/SelectFramework';

class Overview extends React.Component {
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
    return framework === 'Knockout' ? <OverviewKO /> : framework === 'Vue' ? <OverviewVue /> : <OverviewReact />;
  }
}

const OverviewReact = _ => (
  <Article vm="Overview" id="Content">
    <Markdown id="Content">
      <FromScratchLink />
      <HelloWorldCode />
      <RealtimePushCode />
      <Expander label={<SeeItLive />} content={<RealTimePush />} connectOnExpand />
      <ServerUpdateCode />
      <Expander label={<SeeItLive />} content={<ServerUpdate />} />
    </Markdown>
  </Article>
);

const OverviewKO = _ => (
  <Article vm="OverviewKO" id="Content">
    <Markdown id="Content">
      <FromScratchLink framework="Knockout" />
      <Expander label={<SeeItLive />} content={<RealTimePushKO />} connectOnExpand />
      <Expander label={<SeeItLive />} content={<ServerUpdateKO />} />
    </Markdown>
  </Article>
);

const OverviewVue = _ => (
  <Article vm="OverviewVue" id="Content">
    <Markdown id="Content">
      <FromScratchLink framework="Vue" />
      <Expander label={<SeeItLive />} content={<RealTimePushVue />} connectOnExpand />
      <Expander label={<SeeItLive />} content={<ServerUpdateVue />} />
      <Expander label={<SeeItLive />} content={<TwoWayBindingVue />} />
    </Markdown>
  </Article>
);

const SeeItLive = _ => <b>See It Live!</b>;

const HelloWorldClass = `
\`\`\`jsx
import React from 'react';
import dotnetify from 'dotnetify';

class MyApp extends React.Component {
  constructor(props) {
    super(props);
    dotnetify.react.connect("HelloWorld", this);
    this.state = { Greetings: "" };
  }
  render() {
      return <div>{this.state.Greetings}</div>
  }
}
\`\`\`
`;

const HelloWorldHook = `
\`\`\`jsx
import { useConnect } from 'dotnetify';

const MyApp = () => {
   const { state } = useConnect("HelloWorld", { Greetings: "" });
   return <div>{state.Greetings}</div>
}
\`\`\`
`;

const HelloWorldCode = _ => (
  <Tab>
    <TabItem label="Class">
      <Markdown text={HelloWorldClass} />
    </TabItem>
    <TabItem label="Hook">
      <Markdown text={HelloWorldHook} />
    </TabItem>
  </Tab>
);

const RealtimePushClass = `
\`\`\`jsx
class MyApp extends React.Component {
   constructor(props) {
      super(props);
      dotnetify.react.connect("HelloWorld", this);
      this.state = { Greetings: "", ServerTime: "" };
   }
   render() {
      return (
         <div>
            <p>{this.state.Greetings}</p>
            <p>Server time is: {this.state.ServerTime}</p>
         </div>
      );
   }
}
\`\`\`
`;

const RealtimePushHook = `
\`\`\`jsx
const MyApp = () => {
   const { state } = useConnect("HelloWorld", { Greetings: "", ServerTime: "" });
   return (
      <div>
        <p>{state.Greetings}</p>
        <p>Server time is: {state.ServerTime}</p>
      </div>
   );
}
\`\`\`
`;

const RealtimePushCode = _ => (
  <Tab>
    <TabItem label="Class">
      <Markdown text={RealtimePushClass} />
    </TabItem>
    <TabItem label="Hook">
      <Markdown text={RealtimePushHook} />
    </TabItem>
  </Tab>
);

const RealTimePush = () => {
  const { state } = useConnect('RealTimePush', { Greetings: '', ServerTime: '' });
  return (
    <div>
      <p>{state.Greetings}</p>
      <p>Server time is: {state.ServerTime}</p>
    </div>
  );
};

class RealTimePushKO extends React.Component {
  componentDidMount() {
    dotnetify.ko.init();
  }
  render() {
    return (
      <div data-vm="RealTimePush">
        <p data-bind="text: Greetings" />
        <p data-bind="text: ServerTime" />
      </div>
    );
  }
}

class RealTimePushVue extends React.Component {
  componentDidMount() {
    this.app = new Vue({
      template: '<div><p>{{Greetings}}</p><p>Server time is: {{ServerTime}}</p></div>',
      created() {
        this.vm = dotnetify.vue.connect('RealTimePush', this);
      },
      destroyed() {
        this.vm.$destroy();
      },
      data() {
        return { Greetings: '', ServerTime: '' };
      }
    });
    this.app.$mount('#RealTimePushVue');
  }
  componentWillUnmount() {
    this.app.$destroy();
  }
  render() {
    return <div id="RealTimePushVue" />;
  }
}

const ServerUpdateClass = `
\`\`\`jsx
class MyApp extends React.Component {
   constructor(props) {
      super(props);
      this.vm = dotnetify.react.connect("HelloWorld", this);
      this.state = { Greetings: "", firstName: "", lastName: "" };
   }
   render() {
      const handleFirstName = e => this.setState({firstName: e.target.value});
      const handleLastName = e => this.setState({lastName: e.target.value});
      const handleSubmit = () => {
        this.vm.$dispatch({
          Submit: { FirstName: this.state.firstName, LastName: this.state.lastName }
        });
      }
      return (
         <div>
            <div>{this.state.Greetings}</div>
            <input type="text" value={this.state.firstName} onChange={handleFirstName} />
            <input type="text" value={this.state.lastName} onChange={handleLastName} />
            <button onClick={handleSubmit}>Submit</button>
         </div>
      );
   }
}
\`\`\`
`;

const ServerUpdateHook = `
\`\`\`jsx
const MyApp = () => {
  const { vm, state } = useConnect('HelloWorld', { Greetings: '' });
  const [ firstName, setFirstName ] = useState('');
  const [ lastName, setLastName ] = useState('');

  const handleFirstName = e => setFirstName(e.target.value);
  const handleLastName = e => setLastName(e.target.value);
  const handleSubmit = () => {
    vm.$dispatch({ Submit: { FirstName: firstName, LastName: lastName } });
  }
  return (
    <div>
      <div>{state.Greetings}</div>
      <input type="text" value={firstName} onChange={handleFirstName} />
      <input type="text" value={lastName} onChange={handleLastName} />
      <button onClick={handleSubmit}>Submit</button>
    </div>
  );  
}
\`\`\`
`;

const ServerUpdateCode = _ => (
  <Tab>
    <TabItem label="Class">
      <Markdown text={ServerUpdateClass} />
    </TabItem>
    <TabItem label="Hook">
      <Markdown text={ServerUpdateHook} />
    </TabItem>
  </Tab>
);

const ServerUpdate = () => {
  const { vm, state } = useConnect('ServerUpdate', { Greetings: '' });
  const [ firstName, setFirstName ] = useState('');
  const [ lastName, setLastName ] = useState('');

  const handleFirstName = e => setFirstName(e.target.value);
  const handleLastName = e => setLastName(e.target.value);
  const handleSubmit = () => vm.$dispatch({ Submit: { FirstName: firstName, LastName: lastName } });
  return (
    <div>
      <div>{state.Greetings}</div>
      <input type="text" value={firstName} onChange={handleFirstName} />
      <input type="text" value={lastName} onChange={handleLastName} />
      <button onClick={handleSubmit}>Submit</button>
    </div>
  );
};

class ServerUpdateKO extends React.Component {
  componentDidMount() {
    window.ServerUpdate = {
      firstName: ko.observable(),
      lastName: ko.observable(),
      submit: function() {
        this.Submit({ FirstName: this.firstName(), LastName: this.lastName() });
      }
    };
    dotnetify.ko.init();
  }
  render() {
    return (
      <div data-vm="ServerUpdate">
        <div data-bind="text: Greetings" />
        <input type="text" data-bind="value: firstName" />
        <input type="text" data-bind="value: lastName" />
        <button data-bind="vmCommand: submit">Submit</button>
      </div>
    );
  }
}

class ServerUpdateVue extends React.Component {
  componentDidMount() {
    this.app = new Vue({
      template: `
      <div>
        <div>{{Greetings}}</div>
        <input type="text" v-model="firstName" />
        <input type="text" v-model="lastName" />
        <button @click="onSubmit">Submit</button>
      </div>`,
      created() {
        this.vm = dotnetify.vue.connect('ServerUpdate', this);
      },
      destroyed() {
        this.vm.$destroy();
      },
      data() {
        return { Greetings: '', firstName: '', lastName: '' };
      },
      methods: {
        onSubmit() {
          this.vm.$dispatch({ Submit: { FirstName: this.firstName, LastName: this.lastName } });
        }
      }
    });
    this.app.$mount('#ServerUpdateVue');
  }
  componentWillUnmount() {
    this.app.$destroy();
  }
  render() {
    return <div id="ServerUpdateVue" />;
  }
}

class TwoWayBindingVue extends React.Component {
  componentDidMount() {
    this.app = new Vue(
      dotnetify.vue.component(
        {
          name: 'TwoWayBinding',
          template: `
          <div>
            <div>{{state.Greetings}}</div>
            <input type="text" v-model="state.Name" />
          </div>
        `
        },
        'TwoWayBinding',
        { watch: [ 'Name' ] }
      )
    );
    this.app.$mount('#TwoWayBindingVue');
  }
  componentWillUnmount() {
    this.app.$destroy();
  }
  render() {
    return <div id="TwoWayBindingVue" />;
  }
}

export default withTheme(Overview);
