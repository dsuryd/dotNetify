import htmlToReact from 'html-to-react';

export default class WebComponentHelper {
   constructor(host) {
      this.host = host;
      this.host.__eventHandlers = this.host.__eventHandlers || {};
   }

   convertAttributeToProp(componentPropTypes, attrName, attrValue) {
      const propName = Object.keys(componentPropTypes).find(key => key.toLowerCase() == attrName);

      // Convert attribute value type, which is always string, to the expected property type.
      let value = attrValue;
      if (attrName === 'css') value = attrValue;
      else if (attrValue === 'true' || attrValue === 'false') value = attrValue == 'true';
      else if (!isNaN(attrValue) && attrValue !== '') value = +attrValue;
      else if (/^{.*}/.exec(attrValue)) value = JSON.parse(attrValue);
      else if (/([A-z0-9$_]*)\(.*\)/.exec(attrValue)) value = this.parseFunctionString(attrValue);

      return {
         name: propName ? propName : attrName,
         value: value
      };
   }

   getProps(attributes, componentPropTypes) {
      componentPropTypes = componentPropTypes || {};
      return [ ...attributes ]
         .filter(attr => attr.name !== 'style')
         .map(attr => this.convertAttributeToProp(componentPropTypes, attr.name, attr.value))
         .reduce((props, prop) => ({ ...props, [prop.name]: prop.value }), {});
   }

   getEvents(attributes, componentPropTypes) {
      componentPropTypes = componentPropTypes || {};
      // Look for attributes with camel-case names that start with 'on'.
      return Object.keys(componentPropTypes).filter(key => /on([A-Z].*)/.exec(key)).reduce(
         (events, e) => ({
            ...events,
            [e]: args => {
               const eventName = e.toLowerCase();

               let eventHandler = this.host.__eventHandlers[eventName];
               if (!eventHandler) {
                  const attr = [ ...attributes ].find(attr => attr.name == eventName);
                  if (attr) {
                     eventHandler = attr.value;
                     if (/([A-z0-9$_]*)\(.*\)/.exec(attr.value)) eventHandler = this.parseFunctionString(attr.value);
                     this.host.__eventHandlers[eventName] = eventHandler;
                  }
               }

               if (eventHandler && typeof eventHandler !== 'function') eventHandler = this._eval(eventHandler);
               let result = typeof eventHandler == 'function' ? eventHandler(args) : eventHandler;

               this.host.dispatchEvent(new CustomEvent(e, { detail: args }));

               if (typeof this.host.vmContextElem)
                  this.host.vmContextElem.dispatchVMEvent('onElementEvent', {
                     detail: {
                        targetId: this.host.getAttribute('id'),
                        eventName: e,
                        eventArgs: args
                     }
                  });

               return result || null;
            }
         }),
         {}
      );
   }

   getContainerParent() {
      let parent = this.host.parentElement;
      while (parent) {
         if (parent._isContainer) return parent;
         parent = parent.parentElement;
      }
      return null;
   }

   parseFunctionString(funcString) {
      return WebComponentHelper._parseFunctionString(funcString);
   }

   parseHtmlToReact(html) {
      return new htmlToReact.Parser().parse(html.trim());
   }

   static _parseFunctionString(funcString) {
      if (!funcString) return null;
      return args => {
         const result = this._eval(`${funcString}`);
         return typeof result == 'function' ? result(args) : result;
      };
   }

   static _eval(funcString) {
      return Function('"use strict";return (' + funcString + ')')();
   }
}
