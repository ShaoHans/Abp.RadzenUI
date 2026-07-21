using System.Globalization;
using Abp.RadzenUI.Application.Contracts.LocalizationTexts;
using Abp.RadzenUI.Localization;
using Abp.RadzenUI.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Radzen;
using Radzen.Blazor;
using Volo.Abp.AspNetCore.Components.Notifications;

namespace Abp.RadzenUI.Components.Pages.Localization;

public partial class List
{
    [Inject]
    protected ILocalizationTextAppService LocalizationTextAppService { get; set; } = default!;

    [Inject]
    protected DialogService DialogService { get; set; } = default!;

    [Inject]
    protected new IUiNotificationService Notify { get; set; } = default!;

    [Inject]
    public IStringLocalizer<AbpRadzenUIResource> UL { get; set; } = default!;

    private RadzenDataGrid<LocalizationTextItemDto> _grid = default!;
    private IReadOnlyList<LocalizationTextItemDto> _items = [];
    private int _totalCount;
    private bool _isLoading;
    private bool _hasTriggeredInitialLoad;

    private IReadOnlyList<LocalizationResourceInfoDto> _resources = [];
    private IReadOnlyList<LocalizationCultureDto> _cultures = [];
    private string? _selectedResource;
    private string? _selectedCulture;
    private string? _filter;
    private bool _onlyOverridden;

    private readonly int _defaultPageSize = 20;
    private readonly IEnumerable<int> _pageSizeOptions = [10, 20, 30, 50, 100];

    public bool HasEditPermission { get; set; }
    public bool HasDeletePermission { get; set; }
    public bool CanQuery => !string.IsNullOrEmpty(_selectedResource) && !string.IsNullOrEmpty(_selectedCulture);

    public List()
    {
        LocalizationResource = typeof(AbpRadzenUIResource);
    }

    protected override async Task OnInitializedAsync()
    {
        await SetPermissionsAsync();

        _resources = (await LocalizationTextAppService.GetResourcesAsync()).Items;
        _cultures = (await LocalizationTextAppService.GetCulturesAsync()).Items;

        _selectedResource = _resources.FirstOrDefault()?.Name;
        _selectedCulture = ResolveDefaultCulture();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        // The initial resource/culture come from an async load in OnInitializedAsync, so the very
        // first render can happen before CanQuery is true. Trigger the initial load on whichever
        // render CanQuery first becomes true (not only firstRender).
        if (!_hasTriggeredInitialLoad && _grid is not null && CanQuery)
        {
            _hasTriggeredInitialLoad = true;
            await _grid.FirstPage(true);
        }
    }

    private string? ResolveDefaultCulture()
    {
        var current = CultureInfo.CurrentUICulture.Name;
        return _cultures.Any(c => c.CultureName == current)
            ? current
            : _cultures.FirstOrDefault()?.CultureName;
    }

    private async Task SetPermissionsAsync()
    {
        HasEditPermission = await AuthorizationService.IsGrantedAsync(RadzenUIPermissions.Localization.Edit);
        HasDeletePermission = await AuthorizationService.IsGrantedAsync(RadzenUIPermissions.Localization.Delete);
    }

    private async Task ReloadAsync()
    {
        if (_grid is not null && CanQuery)
        {
            await _grid.FirstPage(true);
        }
    }

    private async Task LoadDataAsync(LoadDataArgs args)
    {
        if (!CanQuery)
        {
            _items = [];
            _totalCount = 0;
            return;
        }

        _isLoading = true;
        try
        {
            var result = await LocalizationTextAppService.GetListAsync(
                new GetLocalizationTextsInput
                {
                    ResourceName = _selectedResource!,
                    CultureName = _selectedCulture!,
                    Filter = _filter,
                    OnlyOverridden = _onlyOverridden,
                    Sorting = args.OrderBy,
                    SkipCount = args.Skip ?? 0,
                    MaxResultCount = args.Top ?? _defaultPageSize,
                });

            _items = result.Items;
            _totalCount = (int)result.TotalCount;
        }
        finally
        {
            _isLoading = false;
        }
    }

    private async Task OpenEditDialogAsync(LocalizationTextItemDto item)
    {
        var parameters = new Dictionary<string, object?>
        {
            { nameof(EditLocalizationTextDialog.Item), item },
            { nameof(EditLocalizationTextDialog.CanReset), HasDeletePermission && item.IsOverridden },
        };

        var result = await DialogService.OpenAsync<EditLocalizationTextDialog>(
            L["Localization:EditText"],
            parameters,
            new DialogOptions { Draggable = true, Width = "640px" });

        if (result is not EditLocalizationTextDialog.EditResult editResult)
        {
            return;
        }

        try
        {
            if (editResult.Reset)
            {
                await LocalizationTextAppService.ResetAsync(new ResetLocalizationTextDto
                {
                    ResourceName = item.ResourceName,
                    CultureName = item.CultureName,
                    Key = item.Key,
                });
                await Notify.Success(UL["Localization:ResetSuccessfully"]);
            }
            else
            {
                await LocalizationTextAppService.SaveAsync(new SaveLocalizationTextDto
                {
                    ResourceName = item.ResourceName,
                    CultureName = item.CultureName,
                    Key = item.Key,
                    Value = editResult.Value ?? string.Empty,
                });
                await Notify.Success(UL["SavedSuccessfully"]);
            }

            await _grid.Reload();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }
}
