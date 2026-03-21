window.abpRadzenFullscreen = (() => {
    let dotNetRef = null;
    let fullscreenHandler = null;

    const fullscreenElement = () =>
        document.fullscreenElement || document.webkitFullscreenElement || null;

    const isActive = () => !!fullscreenElement();

    const requestFullscreen = async () => {
        const element = document.documentElement;
        const request = element.requestFullscreen || element.webkitRequestFullscreen;

        if (!request) {
            return false;
        }

        await request.call(element);
        return isActive();
    };

    const exitFullscreen = async () => {
        const exit = document.exitFullscreen || document.webkitExitFullscreen;

        if (!exit) {
            return false;
        }

        await exit.call(document);
        return isActive();
    };

    const dispose = () => {
        if (fullscreenHandler) {
            document.removeEventListener("fullscreenchange", fullscreenHandler);
            document.removeEventListener("webkitfullscreenchange", fullscreenHandler);
            fullscreenHandler = null;
        }

        dotNetRef = null;
    };

    return {
        initialize: function (reference) {
            dispose();
            dotNetRef = reference;
            fullscreenHandler = () => {
                if (dotNetRef) {
                    dotNetRef.invokeMethodAsync("HandleFullscreenChange", isActive());
                }
            };

            document.addEventListener("fullscreenchange", fullscreenHandler);
            document.addEventListener("webkitfullscreenchange", fullscreenHandler);

            return isActive();
        },
        toggle: async function () {
            return isActive() ? await exitFullscreen() : await requestFullscreen();
        },
        dispose: dispose,
        isActive: isActive
    };
})();
