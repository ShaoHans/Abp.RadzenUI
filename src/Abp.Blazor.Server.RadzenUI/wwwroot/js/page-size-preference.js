window.abpRadzenCookie = window.abpRadzenCookie || {
    set: function (name, value, days) {
        if (!name) {
            return;
        }

        const totalDays = Number.isFinite(days) ? Math.trunc(days) : 3650;
        const maxAge = totalDays * 24 * 60 * 60;
        const encodedValue = value === undefined || value === null ? "" : encodeURIComponent(value);

        document.cookie = `${encodeURIComponent(name)}=${encodedValue}; path=/; max-age=${maxAge}; samesite=lax`;
    }
};
