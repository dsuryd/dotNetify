dotnetify_blazor = {
    version: 0.1,
    _eventListeners: [],
    addEventListener: function (event, elem, callbackHelper) {
        if (typeof elem === 'string') elem = document.querySelector(elem);
        if (!(elem && typeof elem.addEventListener === 'function')) throw "Cannot listen to event '" + event + "': invalid ElementRef";

        // Blazor component inside React container must be temporarily moved out until the container is rendered,
        // otherwise it will re-rendered as React children and lose its association with the Blazor app.
        if (elem.getAttribute('vm') && event === 'onStateChange') {
            const insideContainer = () => {
                let parent = elem.parentElement;
                while (parent) {
                    if (parent._isContainer) return parent;
                    parent = parent.parentElement;
                }
            };
            const parent = insideContainer();
            if (parent) {
                const vmId = elem.getAttribute('vm');
                const id = 'tmp__' + vmId;
                const placeholder = document.createElement("span");
                placeholder.setAttribute('id', id);
                elem = elem.parentElement.replaceChild(placeholder, elem);
                elem.setAttribute('vm', '');
                document.body.appendChild(elem);

                let moved = false;
                const restorePosition = () => {
                    if (!moved) {
                        const placeholder = document.getElementById(id);
                        placeholder.parentElement.replaceChild(elem, placeholder);
                        elem.setAttribute('vm', vmId);
                        moved = true;
                    }
                    parent.removeEventListener('mounted', restorePosition);
                };
                parent.addEventListener('mounted', restorePosition);
                setTimeout(restorePosition, 750);
            }
        }

        const callback = e => callbackHelper.invokeMethodAsync('Callback', JSON.stringify(e.detail));
        if (!dotnetify_blazor._eventListeners.some(x => x.elem === elem && x.event === event)) {
            dotnetify_blazor._eventListeners.push({ elem, event, remove: () => elem.removeEventListener(event, callback) });
            elem.addEventListener(event, callback);
        }
    },
    dispatch: function (elem, state) {
        if (!(elem && elem.vm)) throw "ElementRef must reference 'd-vm-context'";
        elem.vm.$dispatch(JSON.parse(state));
    },
    removeAllEventListeners: function (elem) {
        dotnetify_blazor._eventListeners.filter(x => x.elem === elem).forEach(x => x.remove());
        dotnetify_blazor._eventListeners = dotnetify_blazor._eventListeners.filter(x => x.elem !== elem);
    }
};