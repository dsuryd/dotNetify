import React from 'react';
import ReactDOM from 'react-dom';
import dotnetify from 'dotnetify';
import App from './App';
import Dashboard from './dashboard/Dashboard';
import Form from './form/Form';

dotnetify.debug = true;

// Import all the routeable views into the global window variable.
Object.assign(window, { Form, Dashboard });

ReactDOM.render(<App />, document.getElementById('App'));
