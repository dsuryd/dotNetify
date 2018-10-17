import React from 'react';
import SecurePageReact from './react/SecurePage';
import SecurePageVue from './vue/adapters/SecurePage';
import Example from './components/Example';

export default _ => <Example vm="SecurePageExample" react={<SecurePageReact />} vue={<SecurePageVue />} />;
