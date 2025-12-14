window.tabInterop = {
    saveTabs: function (tabsJson) {
        try {
            localStorage.setItem('tabs', tabsJson);
        } catch (e) {
            console.error('Failed to save tabs to localStorage:', e);
        }
    },
    loadTabs: function () {
        try {
            return localStorage.getItem('tabs') || '[]';
        } catch (e) {
            console.error('Failed to load tabs from localStorage:', e);
            return '[]';
        }
    },
    removeTabs: function () {
        try {
            localStorage.removeItem('tabs');
        } catch (e) {
            console.error('Failed to remove tabs from localStorage:', e);
        }
    }
};