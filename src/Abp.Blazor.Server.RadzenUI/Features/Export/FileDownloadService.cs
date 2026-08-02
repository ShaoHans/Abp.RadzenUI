using Microsoft.JSInterop;

namespace Abp.RadzenUI.Features.Export;

/// <summary>
/// Delivers file bytes to the browser via <c>abpRadzenDownload.saveAsFile</c>
/// (see <c>wwwroot/js/file-download.js</c>) using a <see cref="DotNetStreamReference"/>.
/// Scoped, because <see cref="IJSRuntime"/> is bound to the current circuit.
/// </summary>
public class FileDownloadService : IFileDownloadService
{
    private const int StreamBufferSize = 81920;

    private readonly IJSRuntime _jsRuntime;

    public FileDownloadService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task DownloadAsync(
        string fileName,
        byte[] content,
        CancellationToken cancellationToken = default
    )
    {
        using var stream = new MemoryStream(content);
        await SendAsync(fileName, stream, cancellationToken);
    }

    public async Task DownloadFileAsync(
        string fileName,
        string filePath,
        bool deleteAfterDownload = true,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            await using var stream = new FileStream(
                filePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                StreamBufferSize,
                useAsync: true
            );
            await SendAsync(fileName, stream, cancellationToken);
        }
        finally
        {
            if (deleteAfterDownload)
            {
                TryDelete(filePath);
            }
        }
    }

    private async Task SendAsync(string fileName, Stream stream, CancellationToken cancellationToken)
    {
        using var streamRef = new DotNetStreamReference(stream);
        await _jsRuntime.InvokeVoidAsync(
            "abpRadzenDownload.saveAsFile",
            cancellationToken,
            fileName,
            streamRef
        );
    }

    private static void TryDelete(string filePath)
    {
        try
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
        catch
        {
            // Best-effort cleanup of a temp file; ignore failures.
        }
    }
}
