import React from 'react';
import ReactDOM from 'react-dom';
import App from './Views/App';
import './styles.css';
import { enableSsr } from 'dotnetify';

enableSsr();

ReactDOM.hydrate(<App />, document.getElementById('App'));
