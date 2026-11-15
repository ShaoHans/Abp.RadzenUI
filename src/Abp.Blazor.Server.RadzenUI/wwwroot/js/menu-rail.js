// 折叠侧边栏的浮层是 position: fixed，鼠标移入一级图标时按图标位置定位，并避免超出视口底部。
(() => {
    const margin = 8;

    const place = (item, event) => {
        if (event && event.relatedTarget instanceof Node && item.contains(event.relatedTarget)) {
            return;
        }

        const flyout = item.querySelector(':scope > .app-rail__flyout');
        if (!flyout) {
            return;
        }

        const rect = item.getBoundingClientRect();
        const maxTop = window.innerHeight - flyout.offsetHeight - margin;
        const top = Math.max(margin, Math.min(rect.top, maxTop));

        item.style.setProperty('--app-rail-flyout-top', `${top}px`);
        item.style.setProperty('--app-rail-flyout-left', `${rect.right}px`);
    };

    document.addEventListener('mouseover', event => {
        const item = event.target instanceof Element ? event.target.closest('.app-rail__item') : null;
        if (item) {
            place(item, event);
        }
    });

    document.addEventListener('focusin', event => {
        const item = event.target instanceof Element ? event.target.closest('.app-rail__item') : null;
        if (item && !item.contains(event.relatedTarget)) {
            place(item);
        }
    });
})();
