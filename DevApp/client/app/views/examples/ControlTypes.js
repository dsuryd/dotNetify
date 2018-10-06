import React from 'react';
import ControlTypesReact from './react/ControlTypes';
import ControlTypesKo from './knockout/adapters/ControlTypes';
import ControlTypesVue from './vue/adapters/ControlTypes';
import Example from './components/Example';

export default _ => <Example vm="ControlTypesExample" react={<ControlTypesReact />} ko={<ControlTypesKo />} vue={<ControlTypesVue />} />;
