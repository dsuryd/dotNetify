import dotnetify from "./dotnetify-vue";
import "./dotnetify-vue.router";
import Scope from "./Scope";
import { registerDirectives } from "./directives";

dotnetify.vue.Scope = Scope;
module.exports = Object.assign(dotnetify, { Scope, registerDirectives });
