import $ from 'jquery';
import dotnetify from 'dotnetify/dist/dotnetify-ko';
import App from './App.ts';
import 'App.scss';
import SimpleListVM from './SimpleListVM.ts';
import LiveChartVM from './LiveChartVM.ts';

dotnetify.debug = true;

// Set the components to global window to make it accessible to dotNetify routing.
Object.assign(window, { App, SimpleListVM, LiveChartVM });

$('#App').load('App.html', () => dotnetify.ko.init());
