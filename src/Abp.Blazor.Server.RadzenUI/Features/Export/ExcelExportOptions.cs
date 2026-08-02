namespace Abp.RadzenUI.Features.Export;

/// <summary>
/// Describes a single spreadsheet export. Data is pulled <b>page by page</b> through
/// <see cref="PageDataProvider"/> so the whole result set never needs to live in memory at once —
/// the manager streams each page into the file and only one page is resident at a time.
/// The delegate also keeps the mechanism decoupled from any data-access pattern: a CRUD page, a
/// two-panel page, or a plain component all build this the same way.
/// </summary>
/// <typeparam name="T">The row type returned by <see cref="PageDataProvider"/>.</typeparam>
public class ExcelExportOptions<T>
{
    /// <summary>
    /// Fetches one page of rows: <c>(skipCount, maxResultCount, ct)</c>. Called repeatedly with an
    /// advancing <c>skipCount</c> until it returns fewer rows than requested (the last page) or an
    /// empty page. This is where a page plugs in its own paged query — no assumption is made about
    /// how the data is fetched. Required.
    /// <para>A source that cannot page can return everything on the first call and an empty list
    /// afterwards, at the cost of giving up the bounded-memory benefit.</para>
    /// </summary>
    public required Func<int, int, CancellationToken, Task<IReadOnlyList<T>>> PageDataProvider { get; init; }

    /// <summary>
    /// Optional gate run before any data is fetched. Return <c>false</c> to abort silently.
    /// This is the extension point for interactive authorization — e.g. show a verification-code
    /// (captcha) or confirmation dialog and only return <c>true</c> once it passes.
    /// </summary>
    public Func<Task<bool>>? BeforeExportAsync { get; init; }

    /// <summary>
    /// Shapes each fetched page into the value handed to <see cref="IExcelExporter"/>. When null,
    /// pages are exported as-is (property names become headers). Return a list of
    /// <c>Dictionary&lt;string, object?&gt;</c> to emit localized headers and a curated column set.
    /// Applied per page, so the shape must be identical across pages.
    /// </summary>
    public Func<IReadOnlyList<T>, object>? RowSelector { get; init; }

    /// <summary>Download file name. A timestamped default (<c>{Type}-{timestamp}.xlsx</c>) is used when null.</summary>
    public string? FileName { get; init; }

    /// <summary>Worksheet name.</summary>
    public string? SheetName { get; init; }

    /// <summary>Optional ABP policy checked (throwing) before exporting. Null skips the check.</summary>
    public string? PolicyName { get; init; }

    /// <summary>Rows fetched per page. Bounds peak memory. Default 1000.</summary>
    public int PageSize { get; init; } = 1000;

    /// <summary>Overall safety cap on the number of rows exported. Default 1,000,000.</summary>
    public int MaxCount { get; init; } = 1_000_000;

    /// <summary>Show a success notification when the file is produced. Default <c>true</c>.</summary>
    public bool NotifyOnSuccess { get; init; } = true;

    /// <summary>Show an info notification when there is no data to export. Default <c>true</c>.</summary>
    public bool NotifyOnEmpty { get; init; } = true;
}
