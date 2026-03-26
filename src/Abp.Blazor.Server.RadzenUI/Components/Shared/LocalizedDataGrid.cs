using Abp.RadzenUI.Localization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Radzen.Blazor;

namespace Abp.RadzenUI.Components.Shared;

/// <summary>
/// Localization override for RadzenDataGrid.
/// </summary>
/// <typeparam name="TItem">Data type of table records.</typeparam>
[CascadingTypeParameter(nameof(TItem))]
public class LocalizedDataGrid<TItem>(IStringLocalizer<AbpRadzenUIResource> L)
    : RadzenDataGrid<TItem> where TItem : notnull
{
    protected override void OnInitialized()
    {
        base.OnInitialized();

        this.EmptyText = L["DataGrid.EmptyText"];
        this.ApplyFilterText = L["DataGrid:Filter.Apply"];
        this.ClearFilterText = L["DataGrid:Filter.Clear"];
        this.FilterText = L["DataGrid:Filter.Filter"];
        this.EqualsText = L["DataGrid:Filter.Equals"];
        this.NotEqualsText = L["DataGrid:Filter.NotEquals"];
        this.EnumFilterSelectText = L["DataGrid:Filter.Select"];
        this.AndOperatorText = L["DataGrid:Filter.And"];
        this.OrOperatorText = L["DataGrid:Filter.Or"];
        this.ContainsText = L["DataGrid:Filter.Contains"];
        this.DoesNotContainText = L["DataGrid:Filter.DoesNotContain"];
        this.StartsWithText = L["DataGrid:Filter.StartsWith"];
        this.EndsWithText = L["DataGrid:Filter.EndsWith"];
        this.IsNullText = L["DataGrid:Filter.IsNull"];
        this.IsNotNullText = L["DataGrid:Filter.IsNotNull"];
        this.IsEmptyText = L["DataGrid:Filter.IsEmpty"];
        this.IsNotEmptyText = L["DataGrid:Filter.IsNotEmpty"];
        this.LessThanText = L["DataGrid:Filter.LessThan"];
        this.LessThanOrEqualsText = L["DataGrid:Filter.LessThanOrEquals"];
        this.GreaterThanText = L["DataGrid:Filter.GreaterThan"];
        this.GreaterThanOrEqualsText = L["DataGrid:Filter.GreaterThanOrEquals"];
        this.CustomText = L["DataGrid:Filter.Custom"];

        this.PageSizeText = L["DataGrid:Pager.PageSize"];
        this.FirstPageTitle = L["DataGrid:Pager.FirstPageTitle"];
        this.LastPageTitle = L["DataGrid:Pager.LastPageTitle"];
        this.NextPageTitle = L["DataGrid:Pager.NextPageTitle"];
        this.PrevPageTitle = L["DataGrid:Pager.PrevPageTitle"];
        this.PrevPageAriaLabel = L["DataGrid:Pager.PrevPageAriaLabel"];
        this.LastPageAriaLabel = L["DataGrid:Pager.LastPageAriaLabel"];
        this.PageTitleFormat = L["DataGrid:Pager.PageTitleFormat"];
        this.PageAriaLabelFormat = L["DataGrid:Pager.PageAriaLabelFormat"];
        this.PagingSummaryFormat = L["DataGrid:Pager.PagingSummaryFormat"];
        this.FirstPageAriaLabel = L["DataGrid:Pager.FirstPageAriaLabel"];
    }
}
