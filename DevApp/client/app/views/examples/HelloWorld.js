import React from 'react';
import HelloWorldReact from './react/HelloWorld';
import HelloWorldKo from './knockout/HelloWorld';
import Example from './components/Example';

export default _ => <Example vm="HelloWorldExample" react={<HelloWorldReact />} ko={<HelloWorldKo />} />;
