// Streams a byte[] produced on the Blazor Server side to the browser as a file download.
// Invoked from AbpCrudPageBase.DownloadFileAsync via a DotNetStreamReference.
window.abpRadzenDownload = window.abpRadzenDownload || {
    saveAsFile: async function (fileName, contentStreamReference) {
        const arrayBuffer = await contentStreamReference.arrayBuffer();
        const blob = new Blob([arrayBuffer]);
        const url = URL.createObjectURL(blob);

        const anchor = document.createElement('a');
        anchor.href = url;
        anchor.download = fileName || 'download';
        document.body.appendChild(anchor);
        anchor.click();
        anchor.remove();

        URL.revokeObjectURL(url);
    }
};
