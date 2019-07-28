import dotnetify from 'dotnetify';
import App from './App';
import Dashboard from './dashboard/Dashboard';
import Form from './form/Form';
import createWebComponent from './utils/web-component';

dotnetify.debug = true;

// Import all the routeable views into the global window variable.
Object.assign(window, { Form, Dashboard });

createWebComponent(App, 'd-app1');
