window.abpRadzenCommandPalette = (() => {
    let dotNetRef = null;
    let keydownHandler = null;

    const isEditableTarget = (target) => {
        if (!target) {
            return false;
        }
        const tag = target.tagName;
        return tag === "INPUT" || tag === "TEXTAREA" || tag === "SELECT" || target.isContentEditable;
    };

    const dispose = () => {
        if (keydownHandler) {
            document.removeEventListener("keydown", keydownHandler, true);
            keydownHandler = null;
        }
        dotNetRef = null;
    };

    return {
        // Registers a global Ctrl/Cmd+K listener that asks .NET to open the palette.
        initialize: function (reference) {
            dispose();
            dotNetRef = reference;

            keydownHandler = (e) => {
                const isShortcut = (e.ctrlKey || e.metaKey) && !e.altKey && !e.shiftKey
                    && (e.key === "k" || e.key === "K");

                if (!isShortcut) {
                    return;
                }

                // Let the browser keep native Ctrl+K only when nothing to open into.
                e.preventDefault();
                e.stopPropagation();

                if (dotNetRef) {
                    dotNetRef.invokeMethodAsync("OpenFromShortcut");
                }
            };

            // Capture phase so the shortcut works even while focus is inside a field.
            document.addEventListener("keydown", keydownHandler, true);
        },
        // Moves focus into the palette input once it is rendered.
        focus: function (selector) {
            const el = document.querySelector(selector);
            if (el) {
                el.focus();
                if (typeof el.select === "function") {
                    el.select();
                }
            }
        },
        isEditableFocused: function () {
            return isEditableTarget(document.activeElement);
        },
        dispose: dispose
    };
})();
