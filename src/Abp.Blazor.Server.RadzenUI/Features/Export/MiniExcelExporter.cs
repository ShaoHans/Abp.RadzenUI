using System.Collections;
using MiniExcelLibs;

namespace Abp.RadzenUI.Features.Export;

/// <summary>
/// Default <see cref="IExcelExporter"/> backed by
/// <see href="https://github.com/mini-software/MiniExcel">MiniExcel</see> — MIT-licensed,
/// dependency-light and stream-based (low memory), which suits a redistributable UI library.
/// </summary>
public class MiniExcelExporter : IExcelExporter
{
    public const string DefaultSheetName = "Sheet1";

    public async Task<byte[]> ExportAsync(
        object rows,
        string? sheetName = null,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(rows);

        using var stream = new MemoryStream();
        await MiniExcel.SaveAsAsync(
            stream,
            rows,
            sheetName: ResolveSheetName(sheetName),
            excelType: ExcelType.XLSX,
            cancellationToken: cancellationToken
        );

        return stream.ToArray();
    }

    public async Task<long> ExportToFileAsync(
        string filePath,
        IAsyncEnumerable<object> rows,
        string? sheetName = null,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentException.ThrowIfNullOrEmpty(filePath);
        ArgumentNullException.ThrowIfNull(rows);

        var sheet = ResolveSheetName(sheetName);

        // MiniExcel writes a *sync* IEnumerable lazily (row by row) without materializing it — that
        // is what keeps memory bounded. Bridge the async, paged source to that sync enumeration via
        // ToBlockingEnumerable, and run the write on a thread-pool thread (Task.Run) so it is off the
        // Blazor circuit's SynchronizationContext: blocking on the async page fetches then cannot
        // deadlock, because their continuations run freely on the thread pool.
        var countingRows = new CountingEnumerable(rows.ToBlockingEnumerable(cancellationToken));

        await Task.Run(
            () =>
                MiniExcel.SaveAs(
                    filePath,
                    countingRows,
                    printHeader: true,
                    sheetName: sheet,
                    excelType: ExcelType.XLSX
                ),
            cancellationToken
        );

        return countingRows.Count;
    }

    private static string ResolveSheetName(string? sheetName) =>
        string.IsNullOrWhiteSpace(sheetName) ? DefaultSheetName : sheetName;

    /// <summary>
    /// Wraps the row source and counts items as MiniExcel pulls them, so the caller learns the row
    /// count without a second enumeration.
    /// </summary>
    private sealed class CountingEnumerable : IEnumerable<object>
    {
        private readonly IEnumerable<object> _inner;

        public CountingEnumerable(IEnumerable<object> inner) => _inner = inner;

        public long Count { get; private set; }

        public IEnumerator<object> GetEnumerator()
        {
            foreach (var item in _inner)
            {
                Count++;
                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
