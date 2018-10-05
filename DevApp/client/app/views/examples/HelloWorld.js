import React from 'react';
import HelloWorldReact from './react/HelloWorld';
import HelloWorldKo from './knockout/adapters/HelloWorld';
import HelloWorldVue from './vue/adapters/HelloWorld';
import Example from './components/Example';

export default _ => <Example vm="HelloWorldExample" react={<HelloWorldReact />} ko={<HelloWorldKo />} vue={<HelloWorldVue />} />;
