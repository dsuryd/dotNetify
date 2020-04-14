export function setRouteTarget(vm, target) {
  vm.onRouteEnter = (path, template) => {
    template.Target = target;
    return lazyLoad(template.Id);
  };
}

const importPromise = (id, dynamicImport, callback) =>
  new Promise(resolve =>
    dynamicImport.then(module => {
      window[id] = module.default;
      callback && callback(module);
      resolve();
    })
  );

const lazyLoad = id => {
  if (id === 'Home') {
    return importPromise(id, import(/* webpackChunkName: "home" */ './Views/Home'));
  }
  else if (id === 'Page1') {
    return importPromise(id, import(/* webpackChunkName: "page1" */ './Views/Page1'), module => {
      window['Page1A'] = module.Page1A;
      window['Page1B'] = module.Page1B;
    });
  }
  else if (id === 'Page2') {
    return importPromise(id, import(/* webpackChunkName: "page2" */ './Views/Page2'), module => {
      window['Page2Home'] = module.Page2Home;
      window['Page2Item'] = module.Page2Item;
    });
  }
};
