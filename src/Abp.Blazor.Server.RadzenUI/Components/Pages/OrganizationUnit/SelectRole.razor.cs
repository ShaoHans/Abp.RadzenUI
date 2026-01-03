using Abp.RadzenUI.Application.Contracts.Organizations;
using Abp.RadzenUI.Localization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Radzen;
using Radzen.Blazor;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Identity;
using Volo.Abp.Identity.Localization;
using Volo.Abp.Localization;

namespace Abp.RadzenUI.Components.Pages.OrganizationUnit;

public partial class SelectRole
{
    [Parameter]
    public Guid OuId { get; set; }

    [Inject]
    protected IOrganizationUnitAppService AppService { get; set; } = default!;

    [Inject]
    protected DialogService DialogService { get; set; } = default!;

    [Inject]
    public IStringLocalizer<AbpRadzenUIResource> IL { get; set; } = default!;

    protected RadzenDataGrid<IdentityRoleDto> _grid = default!;
    protected IReadOnlyList<IdentityRoleDto> _entities = [];
    protected int _totalCount;
    protected readonly IEnumerable<int> _pageSizeOptions = [10, 20, 30];
    protected readonly bool _showPagerSummary = true;
    protected bool _isLoading = true;
    protected int _defaultPageSize = 10;
    private IList<IdentityRoleDto> _selectedRoles = [];
    private bool? SelectAllValue
    {
        get
        {
            if (_selectedRoles == null || !_selectedRoles.Any())
                return false;

            if (_entities.All(u => _selectedRoles.Contains(u)))
                return true;

            return null;
        }
    }

    protected OrganizationUnitGetUnaddedRoleListInput GetListInput = new();
    protected OrganizationUnitAddUserDto NewEntity;

    public SelectRole()
    {
        NewEntity = new OrganizationUnitAddUserDto();
        LocalizationResource = typeof(IdentityResource);
    }

    protected override async Task OnInitializedAsync()
    {
        await LoadDataAsync(new LoadDataArgs());
    }

    protected virtual async Task LoadDataAsync(LoadDataArgs args)
    {
        _isLoading = true;
        await UpdateGetListInputAsync(args);
        var result = await AppService.GetUnaddedRolesAsync(OuId, GetListInput);
        _entities = result.Items;
        _totalCount = (int)result.TotalCount;
        _isLoading = false;
        StateHasChanged();
    }

    protected virtual Task UpdateGetListInputAsync(LoadDataArgs args)
    {
        if (GetListInput is ISortedResultRequest sortedResultRequestInput)
        {
            sortedResultRequestInput.Sorting = args.OrderBy;
        }

        if (GetListInput is IPagedResultRequest pagedResultRequestInput)
        {
            pagedResultRequestInput.SkipCount = args.Skip ?? 0;
        }

        if (GetListInput is ILimitedResultRequest limitedResultRequestInput)
        {
            limitedResultRequestInput.MaxResultCount = args.Top ?? _defaultPageSize;
        }

        return Task.CompletedTask;
    }

    private async Task SaveAsync()
    {
        try
        {
            if (_selectedRoles.Count == 0)
            {
                DialogService.Close(false);
                return;
            }

            await AppService.AddRolesAsync(
                OuId,
                new OrganizationUnitAddRoleDto { RoleIds = [.. _selectedRoles.Select(u => u.Id)] }
            );
            await Notify.Success(L["SavedSuccessfully"]);
            DialogService.Close(true);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }
}
