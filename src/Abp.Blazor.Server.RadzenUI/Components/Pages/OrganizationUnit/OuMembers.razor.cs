using Abp.RadzenUI.Application.Contracts.Organizations;
using Abp.RadzenUI.Localization;
using Abp.RadzenUI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Radzen;
using Radzen.Blazor;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Identity;
using Volo.Abp.Identity.Localization;
using Volo.Abp.Localization;

namespace Abp.RadzenUI.Components.Pages.OrganizationUnit;

public partial class OuMembers
{
    [Parameter]
    public OrganizationUnitTreeItemVm? SelectedOu { get; set; }

    [Inject]
    protected DialogService DialogService { get; set; } = default!;

    [Inject]
    protected IOrganizationUnitAppService AppService { get; set; } = default!;

    [Inject]
    public IAbpEnumLocalizer AbpEnumLocalizer { get; set; } = default!;

    [Inject]
    public IStringLocalizer<AbpRadzenUIResource> IL { get; set; } = default!;

    protected RadzenDataGrid<IdentityUserDto> _grid = default!;
    protected IReadOnlyList<IdentityUserDto> _entities = [];
    protected int _totalCount;
    protected readonly IEnumerable<int> _pageSizeOptions = [10, 20, 30];
    protected readonly bool _showPagerSummary = true;
    protected bool _isLoading = true;
    protected int _defaultPageSize = 10;

    protected GetIdentityUsersInput GetListInput = new();
    protected OrganizationUnitAddUserDto NewEntity;

    protected string? CreatePolicyName { get; set; }
    protected string? UpdatePolicyName { get; set; }
    protected string? DeletePolicyName { get; set; }

    public bool HasCreatePermission { get; set; }
    public bool HasUpdatePermission { get; set; }
    public bool HasDeletePermission { get; set; }

    public OuMembers()
    {
        NewEntity = new OrganizationUnitAddUserDto();
        LocalizationResource = typeof(IdentityResource);
    }

    protected override async Task OnInitializedAsync() { }

    protected override async Task OnParametersSetAsync()
    {
        if (SelectedOu == null)
        {
            return;
        }
        await TrySetPermissionsAsync();
        await LoadDataAsync(new LoadDataArgs());
    }

    protected virtual async Task LoadDataAsync(LoadDataArgs args)
    {
        _isLoading = true;
        await UpdateGetListInputAsync(args);
        var result = await AppService.GetMembersAsync(SelectedOu!.Id, GetListInput);
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

    private async Task TrySetPermissionsAsync()
    {
        if (IsDisposed)
        {
            return;
        }

        await SetPermissionsAsync();
    }

    protected virtual async Task SetPermissionsAsync()
    {
        if (CreatePolicyName != null)
        {
            HasCreatePermission = await AuthorizationService.IsGrantedAsync(CreatePolicyName);
        }

        if (UpdatePolicyName != null)
        {
            HasUpdatePermission = await AuthorizationService.IsGrantedAsync(UpdatePolicyName);
        }

        if (DeletePolicyName != null)
        {
            HasDeletePermission = await AuthorizationService.IsGrantedAsync(DeletePolicyName);
        }
    }

    private async Task OpenSeleteMemberDialogAsync()
    {
        var parameters = new Dictionary<string, object> { { "OuId", SelectedOu!.Id } };

        var result = await DialogService.OpenAsync<SelectMember>(
            title: IL["Ou:Member.Select", SelectedOu.DisplayName],
            parameters: parameters,
            options: new DialogOptions() { Draggable = true, Width = "700px" }
        );

        if (result == true)
        {
            await _grid.Reload();
        }
    }

    //protected virtual Task<TCreateInput> SetCreateDialogModelAsync()
    //{
    //    return Task.FromResult(new TCreateInput());
    //}

    //protected virtual void CloseDialog()
    //{
    //    DialogService.Close(false);
    //}

    //protected virtual async Task CreateEntityAsync(TCreateInput model)
    //{
    //    try
    //    {
    //        await AppService.AddUsersAsync(SelectedOu.Id, model);
    //        await Notify.Success(UL["SavedSuccessfully"]);
    //        DialogService.Close(true);
    //    }
    //    catch (Exception ex)
    //    {
    //        await HandleErrorAsync(ex);
    //    }
    //}

    private async Task OpenDeleteConfirmDialogAsync(Guid userId, string userName)
    {
        var result = await DialogService.Confirm(
            message: IL["Ou:Member.Delete", SelectedOu!.DisplayName, userName],
            title: "Confirm",
            options: new ConfirmOptions()
            {
                OkButtonText = IL["Yes"],
                CancelButtonText = IL["Cancel"],
            }
        );

        if (result == true)
        {
            try
            {
                await AppService.DeleteUserAsync(SelectedOu!.Id, userId);
                await _grid.Reload();
                await Notify.Success(IL["DeletedSuccessfully"]);
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }
    }

    //protected virtual TCreateInput MapToCreateInput(TCreateViewModel createViewModel)
    //{
    //    if (typeof(TCreateInput) == typeof(TCreateViewModel))
    //    {
    //        return createViewModel.As<TCreateInput>();
    //    }

    //    return ObjectMapper.Map<TCreateViewModel, TCreateInput>(createViewModel);
    //}

    //protected virtual TUpdateInput MapToUpdateInput(TUpdateViewModel updateViewModel)
    //{
    //    if (typeof(TUpdateInput) == typeof(TUpdateViewModel))
    //    {
    //        return updateViewModel.As<TUpdateInput>();
    //    }

    //    return ObjectMapper.Map<TUpdateViewModel, TUpdateInput>(updateViewModel);
    //}

    //protected virtual async Task CheckCreatePolicyAsync()
    //{
    //    await CheckPolicyAsync(CreatePolicyName);
    //}

    //protected virtual async Task CheckUpdatePolicyAsync()
    //{
    //    await CheckPolicyAsync(UpdatePolicyName);
    //}

    //protected virtual async Task CheckDeletePolicyAsync()
    //{
    //    await CheckPolicyAsync(DeletePolicyName);
    //}

    /// <summary>
    /// Calls IAuthorizationService.CheckAsync for the given <paramref name="policyName"/>.
    /// Throws <see cref="AbpAuthorizationException"/> if given policy was not granted for the current user.
    ///
    /// Does nothing if <paramref name="policyName"/> is null or empty.
    /// </summary>
    /// <param name="policyName">A policy name to check</param>
    protected virtual async Task CheckPolicyAsync(string? policyName)
    {
        if (string.IsNullOrEmpty(policyName))
        {
            return;
        }

        await AuthorizationService.CheckAsync(policyName);
    }
}
