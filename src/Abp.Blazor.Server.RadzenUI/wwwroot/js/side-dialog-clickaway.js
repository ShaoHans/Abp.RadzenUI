let clickHandler = null;
let dotNetRef = null;

function getSideDialogElement() {
    return document.querySelector(".rz-dialog-side") || null;
}

function hasRegularDialogOpen() {
    return !!document.querySelector(".rz-dialog:not(.rz-dialog-side)");
}

export function register(dotNet) {
    dotNetRef = dotNet;

    if (clickHandler) {
        return;
    }

    clickHandler = (event) => {
        const dialog = getSideDialogElement();
        if (!dialog) {
            return;
        }

        const target = event.target;
        const element = target instanceof Element ? target : target?.parentElement;

        if (!element) {
            return;
        }

        if (dialog.contains(element)) {
            return;
        }

        if (hasRegularDialogOpen()) {
            return;
        }

        if (element.closest(".side-dialog-ignore") || element.closest("[data-side-dialog-ignore='true']")) {
            return;
        }

        if (dotNetRef) {
            dotNetRef.invokeMethodAsync("CloseFromOutsideClick");
        }
    };

    document.addEventListener("click", clickHandler, true);
}

export function unregister() {
    if (!clickHandler) {
        return;
    }

    document.removeEventListener("click", clickHandler, true);
    clickHandler = null;
    dotNetRef = null;
}
