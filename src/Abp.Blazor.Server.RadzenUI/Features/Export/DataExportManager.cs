using System.Collections;
using System.Globalization;
using System.Runtime.CompilerServices;
using Abp.RadzenUI.Localization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
using Volo.Abp.AspNetCore.Components.Notifications;

namespace Abp.RadzenUI.Features.Export;

/// <inheritdoc cref="IDataExportManager"/>
public class DataExportManager : IDataExportManager
{
    private readonly IExcelExporter _exporter;
    private readonly IFileDownloadService _fileDownloadService;
    private readonly IUiNotificationService _notificationService;
    private readonly IAuthorizationService _authorizationService;
    private readonly IStringLocalizer<AbpRadzenUIResource> _localizer;

    public DataExportManager(
        IExcelExporter exporter,
        IFileDownloadService fileDownloadService,
        IUiNotificationService notificationService,
        IAuthorizationService authorizationService,
        IStringLocalizer<AbpRadzenUIResource> localizer
    )
    {
        _exporter = exporter;
        _fileDownloadService = fileDownloadService;
        _notificationService = notificationService;
        _authorizationService = authorizationService;
        _localizer = localizer;
    }

    public async Task<bool> ExportToExcelAsync<T>(
        ExcelExportOptions<T> options,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(options);

        if (!string.IsNullOrEmpty(options.PolicyName))
        {
            await _authorizationService.CheckAsync(options.PolicyName);
        }

        // Gate: captcha / confirmation / quota check. Aborts silently when it returns false.
        if (options.BeforeExportAsync is not null && !await options.BeforeExportAsync())
        {
            return false;
        }

        // Peek the first page up front: this lets us report "no data" cleanly and guarantees the
        // writer always sees at least one row (needed to infer dictionary headers).
        var firstTake = Math.Min(options.PageSize, options.MaxCount);
        var firstPage =
            firstTake <= 0 ? null : await options.PageDataProvider(0, firstTake, cancellationToken);
        if (firstPage is null || firstPage.Count == 0)
        {
            if (options.NotifyOnEmpty)
            {
                await _notificationService.Info(_localizer["Export:NoData"]);
            }

            return false;
        }

        var tempFilePath = Path.Combine(
            Path.GetTempPath(),
            $"abp-radzen-export-{Guid.NewGuid():N}.xlsx"
        );

        try
        {
            // Rows are pulled page by page and streamed straight into the file: peak memory is a
            // single row, not the whole result set.
            await _exporter.ExportToFileAsync(
                tempFilePath,
                StreamRowsAsync(options, firstPage, cancellationToken),
                options.SheetName,
                cancellationToken
            );

            var fileName = string.IsNullOrWhiteSpace(options.FileName)
                ? BuildDefaultFileName<T>()
                : options.FileName;

            // Streams the temp file back to the browser in chunks, then deletes it.
            await _fileDownloadService.DownloadFileAsync(
                fileName,
                tempFilePath,
                deleteAfterDownload: true,
                cancellationToken
            );

            if (options.NotifyOnSuccess)
            {
                await _notificationService.Success(_localizer["Export:Success"]);
            }

            return true;
        }
        finally
        {
            // Safety net if the temp file was created but never downloaded (e.g. an error in between).
            TryDeleteTempFile(tempFilePath);
        }
    }

    /// <summary>
    /// Flattens the paged source into a lazy row-by-row async stream, starting with the
    /// already-fetched <paramref name="firstPage"/> and continuing to page until a short/empty page
    /// or <see cref="ExcelExportOptions{T}.MaxCount"/> is reached.
    /// </summary>
    private static async IAsyncEnumerable<object> StreamRowsAsync<T>(
        ExcelExportOptions<T> options,
        IReadOnlyList<T> firstPage,
        [EnumeratorCancellation] CancellationToken cancellationToken
    )
    {
        foreach (var row in ShapeRows(options, firstPage))
        {
            yield return row;
        }

        var fetched = firstPage.Count;
        var skipCount = firstPage.Count;

        while (fetched < options.MaxCount)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var take = Math.Min(options.PageSize, options.MaxCount - fetched);
            var page = await options.PageDataProvider(skipCount, take, cancellationToken);

            // An empty page is the only reliable end-of-data signal: a "short" page cannot be
            // trusted because the data source may clamp the requested page size (ABP app services
            // cap MaxResultCount at MaxMaxResultCount, 1000 by default).
            if (page is null || page.Count == 0)
            {
                yield break;
            }

            foreach (var row in ShapeRows(options, page))
            {
                yield return row;
            }

            fetched += page.Count;
            skipCount += page.Count;
        }
    }

    private static IEnumerable<object> ShapeRows<T>(
        ExcelExportOptions<T> options,
        IReadOnlyList<T> page
    )
    {
        var shaped = options.RowSelector is not null ? options.RowSelector(page) : page;
        if (shaped is not IEnumerable enumerable)
        {
            yield break;
        }

        foreach (var item in enumerable)
        {
            if (item is not null)
            {
                yield return item;
            }
        }
    }

    private static string BuildDefaultFileName<T>()
    {
        var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);
        return $"{typeof(T).Name}-{timestamp}.xlsx";
    }

    private static void TryDeleteTempFile(string filePath)
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
            // Best-effort cleanup; ignore failures.
        }
    }
}
