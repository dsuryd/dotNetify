dotnetify_blazor = {
    version: 1.0,
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
                const elemParent = elem.parentElement;
                const tempPlaceholderId = "__tmp_" + vmId;
                const tempPlaceholder = document.createElement("span");
                const tempLocation = document.createElement("span");
                const tempParent = document.body.appendChild(tempLocation);
                const restore = () => {
                    const placeholder = document.getElementById(tempPlaceholderId);
                    if (placeholder) {
                        while (tempParent.childNodes.length > 0)
                            placeholder.parentElement.appendChild(tempParent.childNodes[0]);
                        document.body.removeChild(tempParent);
                        placeholder.parentElement.removeChild(placeholder);
                    }
                    elem.setAttribute('vm', vmId);
                    return true;
                }

                tempPlaceholder.setAttribute("id", tempPlaceholderId);
                tempLocation.style.display = "none";
                elem.setAttribute('vm', '');
                while (elemParent.childNodes.length > 0)
                    tempParent.appendChild(elemParent.childNodes[0]);
                elemParent.appendChild(tempPlaceholder);

                let moved = false;
                const restorePosition = () => {
                    moved = moved || restore();
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