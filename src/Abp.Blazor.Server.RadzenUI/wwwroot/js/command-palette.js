window.abpRadzenCommandPalette = (() => {
    let dotNetRef = null;      // headless host — receives the Ctrl/⌘+K shortcut
    let dialogRef = null;      // open palette dialog — receives navigation keys
    let keydownHandler = null;

    const navKeys = ["ArrowDown", "ArrowUp", "Enter"];

    const dispose = () => {
        if (keydownHandler) {
            document.removeEventListener("keydown", keydownHandler, true);
            keydownHandler = null;
        }
        dotNetRef = null;
        dialogRef = null;
    };

    return {
        // Registers a global Ctrl/Cmd+K listener that asks .NET to open the palette,
        // and (while a dialog is registered) routes navigation keys to it.
        initialize: function (reference) {
            dispose();
            dotNetRef = reference;

            keydownHandler = (e) => {
                // While the palette dialog is open, own the navigation keys in the
                // capture phase so focusable widgets inside (e.g. RadzenTabs headers)
                // never consume Enter/arrows — keeps result navigation focus-independent.
                if (dialogRef && navKeys.includes(e.key)) {
                    e.preventDefault();
                    e.stopPropagation();
                    dialogRef.invokeMethodAsync("HandleKey", e.key);
                    return;
                }

                const isShortcut = (e.ctrlKey || e.metaKey) && !e.altKey && !e.shiftKey
                    && (e.key === "k" || e.key === "K");

                if (!isShortcut) {
                    return;
                }

                e.preventDefault();
                e.stopPropagation();

                if (dotNetRef) {
                    dotNetRef.invokeMethodAsync("OpenFromShortcut");
                }
            };

            // Capture phase so the shortcut/nav keys win over inner components.
            document.addEventListener("keydown", keydownHandler, true);
        },
        // Called by the dialog while it is open, so nav keys route to it.
        setDialog: function (reference) {
            dialogRef = reference;
        },
        clearDialog: function () {
            dialogRef = null;
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
        dispose: dispose
    };
})();
