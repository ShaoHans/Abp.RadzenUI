namespace Abp.RadzenUI.Features.Export;

/// <summary>
/// Orchestrates a spreadsheet export end-to-end: optional permission check → optional pre-export
/// gate → fetch (via the caller's delegate) → shape → serialize → browser download → notify.
/// <para>
/// Deliberately independent of <see cref="AbpCrudPageBase"/> and of any data-access convention:
/// because the data comes from <see cref="ExcelExportOptions{T}.DataProvider"/>, any list page can
/// inject this manager and export in one call — CRUD pages, custom pages, or two-panel pages alike.
/// </para>
/// </summary>
public interface IDataExportManager
{
    /// <summary>
    /// Runs the export described by <paramref name="options"/>.
    /// </summary>
    /// <returns><c>true</c> if a file was produced and downloaded; <c>false</c> if the run was
    /// aborted by the gate or there was no data.</returns>
    Task<bool> ExportToExcelAsync<T>(
        ExcelExportOptions<T> options,
        CancellationToken cancellationToken = default
    );
}
