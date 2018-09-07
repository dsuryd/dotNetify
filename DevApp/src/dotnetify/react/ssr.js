import dotnetify from './dotnetify-react';

// Used by server-side rendering in lieu of connect method.
dotnetify.react.ssrConnect = function(iVMId, iReact, iVMArg) {
	if (window.ssr == null || !window.ssr.hasOwnProperty(iVMId))
		console.error("Server-side rendering requires initial state in 'window.ssr." + iVMId + "'.");

	var self = dotnetify.react;
	var vmState = window.ssr[iVMId];
	var getState = function() {
		return vmState;
	};
	var setState = function(state) {
		vmState = $.extend(vmState, state);
	};
	var options = {
		getState: getState,
		setState: setState,
		vmArg: iVMArg
	};
	var vm = (self.viewModels[iVMId] = new dotnetifyVM(iVMId, iReact, options));

	// Need to be asynch to allow initial state to be processed.
	setTimeout(function() {
		vm.$update(JSON.stringify(window.ssr[iVMId]));
	}, 100);
	return vm;
};

// Used by client-side React component to get server-side rendered initial state.
dotnetify.react.router.ssrState = function(iVMId) {
	if (window.ssr && window.ssr[iVMId] && !window.ssr[iVMId].fetched) {
		window.ssr[iVMId].fetched = true;
		return window.ssr[iVMId];
	}
};

// Called from server to configure server-side rendering.
// iPath - initial URL path.
// iInitialStates - serialized object literal '{ "vmName": {initialState}, ...}'.
// iOverrideRouteFn - function to override routing URLs before the router loads them.
// iCallbackFn - callback after the path is fully routed.
// iTimeout - timeout in milliseconds.
dotnetify.react.router.ssr = function(iPath, iInitialStates, iOverrideRouteFn, iCallbackFn, iTimeout) {
	dotnetify.ssr = true;
	dotnetify.react.router.urlPath = iPath;
	dotnetify.react.router.overrideUrl = iOverrideRouteFn;

	// Insert initial states in the head tag.
	var script = document.createElement('script');
	script.type = 'text/javascript';
	script.text = 'window.ssr=' + iInitialStates + ';';
	var head = document.getElementsByTagName('head')[0];
	if (head) head.insertBefore(script, head.firstChild);
	else console.error('router> document head tag is required for server-side render.');

	var routed = false;
	var fallback = iTimeout
		? setTimeout(function() {
				if (!routed) iCallbackFn();
			}, iTimeout)
		: 0;
	window.addEventListener('dotnetify.routed', function() {
		routed = true;
		if (fallback != 0) clearTimeout(fallback);
		iCallbackFn();
	});

	// Add initial states into the window scope for the server-renderd components.
	window.ssr = JSON.parse(iInitialStates);
};
