import dotnetify from './dotnetify-vue';
import Scope from './Scope';
import './directives';

dotnetify.vue.Scope = Scope;
module.exports = Object.assign(dotnetify, { Scope });
