using System.Collections;
using System.Drawing;
using System.Globalization;
using MiniExcelLibs;
using MiniExcelLibs.OpenXml;

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
        ExcelWriteOptions? writeOptions,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(rows);

        using var stream = new MemoryStream();
        await MiniExcel.SaveAsAsync(
            stream,
            rows,
            sheetName: ResolveSheetName(writeOptions?.SheetName),
            excelType: ExcelType.XLSX,
            configuration: BuildConfiguration(writeOptions),
            cancellationToken: cancellationToken
        );

        return stream.ToArray();
    }

    public async Task<long> ExportToFileAsync(
        string filePath,
        IAsyncEnumerable<object> rows,
        ExcelWriteOptions? writeOptions,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentException.ThrowIfNullOrEmpty(filePath);
        ArgumentNullException.ThrowIfNull(rows);

        var sheet = ResolveSheetName(writeOptions?.SheetName);
        var configuration = BuildConfiguration(writeOptions);

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
                    excelType: ExcelType.XLSX,
                    configuration: configuration
                ),
            cancellationToken
        );

        return countingRows.Count;
    }

    private static string ResolveSheetName(string? sheetName) =>
        string.IsNullOrWhiteSpace(sheetName) ? DefaultSheetName : sheetName;

    /// <summary>
    /// Translates the engine-agnostic <see cref="ExcelWriteOptions"/> into MiniExcel's
    /// configuration. Anything the options leave alone is left at MiniExcel's own default, so the
    /// produced file is byte-for-byte what it was before this type existed.
    /// <para>
    /// None of these settings makes MiniExcel enumerate the rows twice (verified), which matters
    /// because the row source is a one-shot bridge over a paged query.
    /// </para>
    /// </summary>
    private static OpenXmlConfiguration BuildConfiguration(ExcelWriteOptions? writeOptions)
    {
        writeOptions ??= new ExcelWriteOptions();

        var configuration = new OpenXmlConfiguration();

        // MiniExcel drives cell borders *and* the header fill from this one switch, and ignores
        // StyleOptions.HeaderStyle completely while it is off — so a caller asking for header
        // styling has to get the tabular look, otherwise their setting would silently do nothing.
        var wantsHeaderStyle =
            writeOptions.HeaderBackgroundColor is not null || writeOptions.HeaderWrapText;
        configuration.TableStyles =
            writeOptions.SheetStyle == ExcelSheetStyle.Tabular || wantsHeaderStyle
                ? TableStyles.Default
                : TableStyles.None;

        configuration.FreezeRowCount = writeOptions.FreezeRowCount;
        configuration.FreezeColumnCount = writeOptions.FreezeColumnCount;

        if (writeOptions.AutoFitColumns)
        {
            // MiniExcel throws "Auto width requires fast mode to be enabled" when EnableAutoWidth is
            // set on its own, so the two always travel together.
            configuration.EnableAutoWidth = true;
            configuration.FastMode = true;
            configuration.MinWidth = writeOptions.MinColumnWidth;
            configuration.MaxWidth = writeOptions.MaxColumnWidth;
        }

        // Only touch the header style when something was actually asked for. MiniExcel leaves
        // StyleOptions.HeaderStyle null and applies its own defaults in that case, so one has to be
        // built here — a fresh OpenXmlHeaderStyle already carries MiniExcel's default header fill
        // (#4472C4 at 40 alpha), which keeps the familiar look when only WrapText was requested.
        if (wantsHeaderStyle)
        {
            var headerStyle = new OpenXmlHeaderStyle { WrapText = writeOptions.HeaderWrapText };
            if (TryParseHexColor(writeOptions.HeaderBackgroundColor, out var color))
            {
                headerStyle.BackgroundColor = color;
            }

            configuration.StyleOptions.HeaderStyle = headerStyle;
        }

        return configuration;
    }

    /// <summary>Parses <c>#RRGGBB</c> / <c>RRGGBB</c>. Returns false for null, blank or malformed input.</summary>
    private static bool TryParseHexColor(string? value, out Color color)
    {
        color = default;
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var hex = value.AsSpan().TrimStart('#');
        if (hex.Length != 6)
        {
            return false;
        }

        if (!int.TryParse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var rgb))
        {
            return false;
        }

        // Fully opaque: the option takes #RRGGBB, so the caller gets exactly that colour. (MiniExcel's
        // own default header fill is the same blue at 40 alpha, i.e. noticeably lighter.)
        color = Color.FromArgb(byte.MaxValue, (rgb >> 16) & 0xFF, (rgb >> 8) & 0xFF, rgb & 0xFF);
        return true;
    }

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
