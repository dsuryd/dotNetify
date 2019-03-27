/* 
Copyright 2017-2018 Dicky Suryadi

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
 */
import $ from '../libs/jquery-shim';

// Client-side view model that acts as a proxy of the server view model.
export default class dotnetifyVM {
  // iVMId - identifies the view model.
  // iComponent - component object.
  // iOptions - Optional configuration options:
  //    getState: state accessor.
  //    setState: state mutator.
  //    vmArg: view model arguments.
  //    headers: request headers, for things like authentication token.
  // iDotNetify - framework-specific dotNetify library.
  constructor(iVMId, iComponent, iOptions, iDotNetify) {
    this.$vmId = iVMId;
    this.$component = iComponent;
    this.$vmArg = iOptions && iOptions['vmArg'];
    this.$headers = iOptions && iOptions['headers'];
    this.$exceptionHandler = iOptions && iOptions['exceptionHandler'];
    this.$requested = false;
    this.$loaded = false;
    this.$itemKey = {};
    this.$dotnetify = iDotNetify;

    let getState = iOptions && iOptions['getState'];
    let setState = iOptions && iOptions['setState'];
    getState = typeof getState === 'function' ? getState : () => iComponent.state;
    setState = typeof setState === 'function' ? setState : state => iComponent.setState(state);

    this.State = state => (typeof state === 'undefined' ? getState() : setState(state));
    this.Props = prop => this.$component.props && this.$component.props[prop];

    const vmArg = this.Props('vmArg');
    if (vmArg) this.$vmArg = $.extend(this.$vmArg, vmArg);

    // Inject plugin functions into this view model.
    this.$getPlugins().map(plugin => (typeof plugin['$inject'] == 'function' ? plugin.$inject(this) : null));
  }

  // Disposes the view model, both here and on the server.
  $destroy() {
    // Call any plugin's $destroy function if provided.
    this.$getPlugins().map(plugin => (typeof plugin['$destroy'] == 'function' ? plugin.$destroy.apply(this) : null));

    const controller = this.$dotnetify.controller;
    if (controller.isConnected()) {
      try {
        controller.disposeVM(this.$vmId);
      } catch (ex) {
        controller._triggerConnectionStateEvent('error', ex);
      }
    }

    delete this.$dotnetify.viewModels[this.$vmId];
  }

  // Dispatches a value to the server view model.
  // iValue - Vvalue to dispatch.
  $dispatch(iValue) {
    const controller = this.$dotnetify.controller;
    if (controller.isConnected()) {
      try {
        controller.updateVM(this.$vmId, iValue);

        if (controller.debug) {
          console.log('[' + this.$vmId + '] sent> ');
          console.log(iValue);

          controller.debugFn && controller.debugFn(this.$vmId, 'sent', iValue);
        }
      } catch (ex) {
        controller._triggerConnectionStateEvent('error', ex);
      }
    }
  }

  // Dispatches a state value to the server view model.
  // iValue - State value to dispatch.
  $dispatchListState(iValue) {
    for (var listName in iValue) {
      const key = this.$itemKey[listName];
      if (!key) {
        console.error(`[${this.$vmId}] missing item key for '${listName}'; add ${listName}_itemKey property to the view model.`);
        return;
      }
      var item = iValue[listName];
      if (!item[key]) {
        console.error(`[${this.$vmId}] couldn't dispatch data from '${listName}' due to missing property '${key}'.`);
        console.error(item);
        return;
      }

      Object.keys(item).forEach(prop => {
        if (prop != key) {
          let state = {};
          state[listName + '.$' + item[key] + '.' + prop] = item[prop];
          this.$dispatch(state);
        }
      });
      this.$updateList(listName, item);
    }
  }

  $getPlugins() {
    return Object.keys(this.$dotnetify.plugins).map(id => this.$dotnetify.plugins[id]);
  }

  // Preprocess view model update from the server before we set the state.
  $preProcess(iVMUpdate) {
    const vm = this;

    for (var prop in iVMUpdate) {
      // Look for property that end with '_add'. Interpret the value as a list item to be added
      // to an existing list whose property name precedes that suffix.
      var match = /(.*)_add/.exec(prop);
      if (match != null) {
        var listName = match[1];
        if (Array.isArray(this.State()[listName])) vm.$addList(listName, iVMUpdate[prop]);
        else console.error('unable to resolve ' + prop);
        delete iVMUpdate[prop];
        continue;
      }

      // Look for property that end with '_update'. Interpret the value as a list item to be updated
      // to an existing list whose property name precedes that suffix.
      var match = /(.*)_update/.exec(prop);
      if (match != null) {
        var listName = match[1];
        if (Array.isArray(this.State()[listName])) vm.$updateList(listName, iVMUpdate[prop]);
        else console.error('[' + this.$vmId + "] '" + listName + "' is not found or not an array.");
        delete iVMUpdate[prop];
        continue;
      }

      // Look for property that end with '_remove'. Interpret the value as a list item key to remove
      // from an existing list whose property name precedes that suffix.
      var match = /(.*)_remove/.exec(prop);
      if (match != null) {
        var listName = match[1];
        if (Array.isArray(this.State()[listName])) {
          var key = this.$itemKey[listName];
          if (key != null)
            vm.$removeList(listName, function(i) {
              return i[key] == iVMUpdate[prop];
            });
          else console.error(`[${this.$vmId}] missing item key for '${listName}'; add ${listName}_itemKey property to the view model.`);
        }
        else console.error(`[${this.$vmId}] '${listName}' is not found or not an array.`);
        delete iVMUpdate[prop];
        continue;
      }

      // Look for property that end with '_itemKey'. Interpret the value as the property name that will
      // uniquely identify items in the list.
      var match = /(.*)_itemKey/.exec(prop);
      if (match != null) {
        var listName = match[1];
        var itemKey = {};
        itemKey[listName] = iVMUpdate[prop];
        vm.$setItemKey(itemKey);
        delete iVMUpdate[prop];
        continue;
      }
    }
  }

  // Requests state from the server view model.
  $request() {
    const controller = this.$dotnetify.controller;
    if (controller.isConnected()) {
      controller.requestVM(this.$vmId, { $vmArg: this.$vmArg, $headers: this.$headers });
      this.$requested = true;
    }
  }

  // Updates state from the server view model to the view.
  // iVMData - Serialized state from the server.
  $update(iVMData) {
    const controller = this.$dotnetify.controller;
    if (controller.debug) {
      console.log('[' + this.$vmId + '] received> ');
      console.log(JSON.parse(iVMData));

      controller.debugFn && controller.debugFn(this.$vmId, 'received', JSON.parse(iVMData));
    }
    var vmData = JSON.parse(iVMData);
    this.$preProcess(vmData);

    var state = this.State();
    state = $.extend({}, state, vmData);
    this.State(state);

    if (!this.$loaded) this.$onLoad();
  }

  // Handles initial view model load event.
  $onLoad() {
    // Call any plugin's $ready function if provided to give a chance to do
    // things when the view model is ready.
    this.$getPlugins().map(plugin => (typeof plugin['$ready'] == 'function' ? plugin.$ready.apply(this) : null));
    this.$loaded = true;
  }

  // *** CRUD Functions ***

  // Sets items key to identify individual items in a list.
  // Accepts object literal: { "<list name>": "<key prop name>", ... }
  $setItemKey(iItemKey) {
    Object.assign(this.$itemKey, iItemKey);
  }

  //// Adds a new item to a state array.
  $addList(iListName, iNewItem) {
    // Check if the list already has an item with the same key. If so, replace it.
    var key = this.$itemKey[iListName];
    if (key != null) {
      if (!iNewItem.hasOwnProperty(key)) {
        console.error(`[${this.$vmId}] couldn't add item to '${iListName}' due to missing property '${key}'.`);
        return;
      }
      var match = this.State()[iListName].filter(function(i) {
        return i[key] == iNewItem[key];
      });
      if (match.length > 0) {
        console.error(`[${this.$vmId}] couldn't add item to '${iListName}' because the key already exists.`);
        return;
      }
    }

    let items = this.State()[iListName];
    items.push(iNewItem);

    let state = {};
    state[iListName] = items;
    this.State(state);
  }

  // Removes an item from a state array.
  $removeList(iListName, iFilter) {
    let state = {};
    state[iListName] = this.State()[iListName].filter(i => !iFilter(i));
    this.State(state);
  }

  //// Updates existing item to an observable array.
  $updateList(iListName, iNewItem) {
    // Check if the list already has an item with the same key. If so, update it.
    let key = this.$itemKey[iListName];
    if (key != null) {
      if (!iNewItem.hasOwnProperty(key)) {
        console.error(`[${this.$vmId}] couldn't update item to '${iListName}' due to missing property '${key}'.`);
        return;
      }
      var state = {};
      state[iListName] = this.State()[iListName].map(function(i) {
        return i[key] == iNewItem[key] ? $.extend(i, iNewItem) : i;
      });
      this.State(state);
    }
    else console.error(`[${this.$vmId}] missing item key for '${iListName}'; add '${iListName}_itemKey' property to the view model.`);
  }
}
