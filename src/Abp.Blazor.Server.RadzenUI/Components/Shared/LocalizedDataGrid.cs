using System.Globalization;
using System.Reflection;
using Abp.RadzenUI.Infrastructure.Services;
using Abp.RadzenUI.Infrastructure.Utils;
using Abp.RadzenUI.Localization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;

namespace Abp.RadzenUI.Components.Shared;

/// <summary>What all <see cref="LocalizedDataGrid{TItem}"/> share, usable without naming a TItem.</summary>
public static class LocalizedDataGrid
{
    /// <summary>The page sizes every list offers. Keep to it: the picked size is shared by all lists.</summary>
    public static IReadOnlyList<int> DefaultPageSizeOptions { get; } = Array.AsReadOnly([10, 20, 30, 50, 100]);
}

/// <summary>
/// Localization override for RadzenDataGrid. A paged grid also reads and writes the page size
/// cookie that is shared by all lists.
/// </summary>
/// <typeparam name="TItem">Data type of table records.</typeparam>
[CascadingTypeParameter(nameof(TItem))]
public class LocalizedDataGrid<TItem>
    : RadzenDataGrid<TItem> where TItem : notnull
{
    // Where Radzen keeps the size picked in the pager. It survives the re-renders of the page, unlike
    // the PageSize parameter, and there is no public way to set it before the first render.
    private static readonly FieldInfo? PickedPageSize = typeof(PagedDataBoundComponent<TItem>)
        .GetField("pageSize", BindingFlags.Instance | BindingFlags.NonPublic);

    private bool _settingParameters;
    private bool _checkFirstPage;
    private int? _appliedPageSize;

    public LocalizedDataGrid()
    {
        // Radzen defaults the pager's page-size dropdown to InputSize.Medium, and
        // Density.Compact only shrinks the page buttons - next to them the dropdown
        // looks oversized. Default it to Small to match the compact list style.
        // Set in the constructor (not OnInitialized) so an explicit
        // PagerInputSize from a page still wins.
        PagerInputSize = InputSize.Small;

        // Every list uses the compact density (smaller rows and pager buttons);
        // defaulting it here keeps dialogs and new grids from falling back to the
        // large Radzen default. Still overridable per page.
        Density = Density.Compact;

        // Every paged list offers the same page sizes, as the picked one is shared by all of them.
        // Overridable per page; PageSizeOptions="null" hides the selector and stops following it.
        PageSizeOptions = LocalizedDataGrid.DefaultPageSizeOptions;
    }

    [Inject]
    private IStringLocalizer<AbpRadzenUIResource> L { get; set; } = default!;

    // The circuit's value: HttpContext keeps the cookies the circuit connected with.
    [Inject]
    private GridPageSizePreferenceService PageSizePreference { get; set; } = default!;

    [Inject]
    private IHttpContextAccessor HttpContextAccessor { get; set; } = default!;

    [Inject]
    private IJSRuntime Js { get; set; } = default!;

    // Without paging or a selector there is nothing to share.
    private bool SharesPageSize => AllowPaging && PageSizeOptions?.Any() == true;

    private int? SharedPageSize =>
        PageSizePreference.PageSize ?? AbpRadzenUICookieHelper.GetPageSizeCookie(HttpContextAccessor.HttpContext);

    public override Task SetParametersAsync(ParameterView parameters)
    {
        // Radzen also calls OnPageSizeChanged for the parameter; only the pager is the user's choice.
        _settingParameters = true;
        try
        {
            return base.SetParametersAsync(parameters);
        }
        finally
        {
            _settingParameters = false;
        }
    }

    protected override async Task OnPageSizeChanged(int value)
    {
        if (_settingParameters || !SharesPageSize)
        {
            await base.OnPageSizeChanged(value);
            return;
        }

        PageSizePreference.PageSize = _appliedPageSize = value;
        var reload = base.OnPageSizeChanged(value);

        try
        {
            await Js.InvokeVoidAsync(
                "abpRadzenCookie.set",
                AbpRadzenUICookieHelper.PageSizeKey,
                value.ToString(CultureInfo.InvariantCulture),
                3650
            );
        }
        catch (JSDisconnectedException)
        {
            // The browser is gone, there is nowhere to write to.
        }

        await reload;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender && _appliedPageSize is not null)
        {
            // Radzen ignores PageSize changes until the second render, which would leave the restored size stuck.
            StateHasChanged();
        }

        // Another list on the same page changed it.
        if (SharesPageSize && SharedPageSize is { } shared && shared != _appliedPageSize && PageSizeOptions!.Contains(shared))
        {
            _appliedPageSize = shared;
            PickedPageSize?.SetValue(this, shared);
            await FirstPage(true);
            StateHasChanged();
        }
        // The page queried before the grid existed, so with its own default.
        else if (_checkFirstPage && Count > 0 && Data is { } data)
        {
            _checkFirstPage = false;

            if (data.Count() != Math.Min(Count, PageSize))
            {
                await Reload();
            }
        }
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        if (SharesPageSize && SharedPageSize is { } saved && PageSizeOptions!.Contains(saved))
        {
            _appliedPageSize = saved;
            _checkFirstPage = saved != PageSize && LoadData.HasDelegate;
            PickedPageSize?.SetValue(this, saved);
        }

        Responsive = true;

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
