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
                // An IME is composing (Safari reports its last key only through keyCode 229):
                // no preventDefault, the key is the IME's. While the palette is open, also keep it
                // from Radzen's handlers, or the Esc that cancels composing closes the dialog.
                if (e.isComposing || e.keyCode === 229) {
                    if (dialogRef) {
                        e.stopPropagation();
                    }
                    return;
                }

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
        // Moves focus into the palette input (on open and after a tab switch). No select():
        // typing after a tab switch continues the keyword instead of replacing it.
        focus: function (selector) {
            const el = document.querySelector(selector);
            if (el) {
                el.focus();
            }
        },
        dispose: dispose
    };
})();
