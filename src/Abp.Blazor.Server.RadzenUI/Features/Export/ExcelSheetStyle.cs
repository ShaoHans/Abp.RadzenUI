namespace Abp.RadzenUI.Features.Export;

/// <summary>
/// The overall look of an exported sheet. Cell borders and the tinted header fill are driven by a
/// single switch in the underlying engine, so they are on or off together — there is no "borderless
/// but with a coloured header" middle ground.
/// </summary>
public enum ExcelSheetStyle
{
    /// <summary>
    /// Plain sheet: no cell borders, no header fill. The default — a wide export reads better
    /// without a grid drawn around every cell, and recipients who want a table look usually apply
    /// their own (Excel's "Format as Table" is two clicks).
    /// </summary>
    Plain,

    /// <summary>
    /// Spreadsheet-table look: tinted header fill plus thin borders around every cell, header and
    /// data alike. Required if <see cref="ExcelWriteOptions.HeaderBackgroundColor"/> or
    /// <see cref="ExcelWriteOptions.HeaderWrapText"/> should have any effect.
    /// </summary>
    Tabular,
}
