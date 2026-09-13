namespace Abp.RadzenUI.Features.Export;

/// <summary>
/// Engine-agnostic description of <b>how</b> a sheet is written: the worksheet name plus the
/// presentation bits a reader needs once the sheet gets wide or long — frozen panes, column widths
/// and the header look. Deliberately exposes no engine type, so replacing
/// <see cref="IExcelExporter"/> still leaves pages untouched.
/// <para>
/// Defaults match the previous behaviour except for <see cref="SheetStyle"/>, which now produces a
/// plain sheet instead of a bordered one — see that member.
/// </para>
/// </summary>
public record ExcelWriteOptions
{
    /// <summary>Worksheet name. The exporter's own default is used when null or blank.</summary>
    public string? SheetName { get; init; }

    /// <summary>
    /// Overall look: plain (default) or the bordered spreadsheet-table look.
    /// <para>
    /// <b>Changed in 2.10.0:</b> the default used to be the bordered look — every cell got a thin
    /// border and the header a blue fill. Set <see cref="ExcelSheetStyle.Tabular"/> to keep that.
    /// </para>
    /// </summary>
    public ExcelSheetStyle SheetStyle { get; init; } = ExcelSheetStyle.Plain;

    /// <summary>
    /// Number of rows frozen at the top. 1 (default) keeps the header row visible while scrolling
    /// down; 0 freezes nothing.
    /// </summary>
    public int FreezeRowCount { get; init; } = 1;

    /// <summary>
    /// Number of columns frozen at the left, so the columns that identify a row stay visible while
    /// scrolling a wide sheet sideways. Default 0 (nothing frozen) — set it to the number of
    /// leading identity columns, e.g. 3 for "code / name / country".
    /// </summary>
    public int FreezeColumnCount { get; init; }

    /// <summary>
    /// Size each column to its content instead of leaving every column at the engine's fixed
    /// default width. Default <c>false</c> (unchanged behaviour). Widths are clamped to
    /// <see cref="MinColumnWidth"/> / <see cref="MaxColumnWidth"/> so one long free-text column
    /// cannot push everything else off screen.
    /// </summary>
    public bool AutoFitColumns { get; init; }

    /// <summary>Lower bound for auto-fitted widths, in Excel character units. Ignored unless <see cref="AutoFitColumns"/>.</summary>
    public double MinColumnWidth { get; init; } = 8;

    /// <summary>Upper bound for auto-fitted widths, in Excel character units. Ignored unless <see cref="AutoFitColumns"/>.</summary>
    public double MaxColumnWidth { get; init; } = 50;

    /// <summary>
    /// Header background as <c>#RRGGBB</c>. Null (default) keeps the engine's own header colour.
    /// <para>
    /// Implies <see cref="ExcelSheetStyle.Tabular"/> — the engine ignores header styling on a plain
    /// sheet, so setting a colour also brings back the cell borders.
    /// </para>
    /// </summary>
    public string? HeaderBackgroundColor { get; init; }

    /// <summary>
    /// Wrap the header text instead of letting a long header run under the next cell.
    /// Default <c>false</c>. Implies <see cref="ExcelSheetStyle.Tabular"/>, like
    /// <see cref="HeaderBackgroundColor"/>.
    /// </summary>
    public bool HeaderWrapText { get; init; }
}
