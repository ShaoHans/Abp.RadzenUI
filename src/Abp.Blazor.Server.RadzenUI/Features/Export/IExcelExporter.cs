namespace Abp.RadzenUI.Features.Export;

/// <summary>
/// Serializes rows into a spreadsheet. Pure and UI-agnostic: it knows nothing about where the data
/// came from or how it will be delivered — so it can be reused by any code, not just list pages.
/// The default implementation is <see cref="MiniExcelExporter"/> (MIT-licensed, streaming, low
/// memory). Swap the engine (ClosedXML / NPOI / ...) via <c>Services.Replace</c> without touching
/// any page.
/// </summary>
public interface IExcelExporter
{
    /// <summary>
    /// In-memory serialization: turns a fully-materialized <paramref name="rows"/> set into a byte
    /// array. Convenient for small result sets. For large exports prefer
    /// <see cref="ExportToFileAsync"/>, which keeps memory bounded.
    /// </summary>
    /// <param name="rows">
    /// An <c>IEnumerable</c> of POCOs (property names become headers) or of
    /// <c>IDictionary&lt;string, object&gt;</c> (keys become headers — use for localized headers).
    /// </param>
    Task<byte[]> ExportAsync(
        object rows,
        string? sheetName = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Streaming serialization: consumes <paramref name="rows"/> lazily and writes them to
    /// <paramref name="filePath"/> as it goes, so peak memory is bounded by a single row rather than
    /// the whole result set. Each item is one row (a POCO, or an <c>IDictionary&lt;string, object&gt;</c>
    /// for localized headers); every item must share the same shape, and the header is taken from the
    /// first one.
    /// </summary>
    /// <returns>The total number of rows written (0 when the source produced no rows).</returns>
    Task<long> ExportToFileAsync(
        string filePath,
        IAsyncEnumerable<object> rows,
        string? sheetName = null,
        CancellationToken cancellationToken = default
    );
}
