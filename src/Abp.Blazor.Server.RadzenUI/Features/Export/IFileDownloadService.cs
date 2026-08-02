namespace Abp.RadzenUI.Features.Export;

/// <summary>
/// Streams an in-memory or on-disk file to the browser as a download. A general-purpose primitive
/// for any interactive Blazor Server component — not tied to exporting or to <see cref="AbpCrudPageBase"/>.
/// </summary>
public interface IFileDownloadService
{
    /// <summary>Downloads an in-memory byte array. Suitable for small payloads.</summary>
    Task DownloadAsync(
        string fileName,
        byte[] content,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Downloads a file from disk by streaming it (chunked over the circuit via
    /// <c>DotNetStreamReference</c>), so server memory for the transfer stays bounded regardless of
    /// file size. When <paramref name="deleteAfterDownload"/> is <c>true</c> the file is removed once
    /// the transfer completes — the intended use for temporary export files.
    /// </summary>
    Task DownloadFileAsync(
        string fileName,
        string filePath,
        bool deleteAfterDownload = true,
        CancellationToken cancellationToken = default
    );
}
