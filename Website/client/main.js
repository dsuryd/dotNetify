import React from 'react';
import ReactDOM from 'react-dom';
import dotnetify from 'dotnetify';
import App from './App';
import 'dotnetify-elements/dotnetify-elements.css';

dotnetify.debug = true;

// Import all the routeable views into the global window variable.
//Object.assign(window, { });

ReactDOM.render(<App />, document.getElementById('App'));
