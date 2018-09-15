import $ from 'jquery';
import dotnetify from 'dotnetify/dist/dotnetify-ko';
import HelloWorld from './HelloWorld.ts';

window.HelloWorld = HelloWorld;

$('#App').load('HelloWorld.html', () => dotnetify.ko.init());
