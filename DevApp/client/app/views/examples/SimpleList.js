import React from 'react';
import SimpleListReact from './react/SimpleList';
import SimpleListKo from './knockout/adapters/SimpleList';
import SimpleListVue from './vue/adapters/SimpleList';
import Example from './components/Example';

export default _ => <Example vm="SimpleListExample" react={<SimpleListReact />} ko={<SimpleListKo />} vue={<SimpleListVue />} />;
