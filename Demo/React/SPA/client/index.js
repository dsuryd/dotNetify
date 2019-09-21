import React from 'react';
import dotnetify from 'dotnetify';
import ReactDOM from 'react-dom';
import App from './App';
import SimpleList from './SimpleList';
import LiveChart from './LiveChart';
import CompositeView from './CompositeView';

dotnetify.debug = true;

// Set the components to global window to make it accessible to dotNetify routing.
Object.assign(window, { SimpleList, LiveChart, CompositeView });

ReactDOM.render(<App />, document.getElementById('App'));
