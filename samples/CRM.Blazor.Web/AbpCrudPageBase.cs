using CRM.Blazor.Web.Models;
using Localization.Resources.AbpUi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Radzen;
using Radzen.Blazor;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.AspNetCore.Components;
using Volo.Abp.Authorization;
using Volo.Abp.Localization;

namespace CRM.Blazor.Web;

public abstract class AbpCrudPageBase<TAppService, TEntityDto, TKey>
    : AbpCrudPageBase<TAppService, TEntityDto, TKey, PagedAndSortedResultRequestDto>
    where TAppService : ICrudAppService<TEntityDto, TKey>
    where TEntityDto : class, IEntityDto<TKey>, new() { }

public abstract class AbpCrudPageBase<TAppService, TEntityDto, TKey, TGetListInput>
    : AbpCrudPageBase<TAppService, TEntityDto, TKey, TGetListInput, TEntityDto>
    where TAppService : ICrudAppService<TEntityDto, TKey, TGetListInput>
    where TEntityDto : class, IEntityDto<TKey>, new()
    where TGetListInput : new() { }

public abstract class AbpCrudPageBase<TAppService, TEntityDto, TKey, TGetListInput, TCreateInput>
    : AbpCrudPageBase<TAppService, TEntityDto, TKey, TGetListInput, TCreateInput, TCreateInput>
    where TAppService : ICrudAppService<TEntityDto, TKey, TGetListInput, TCreateInput>
    where TEntityDto : IEntityDto<TKey>
    where TCreateInput : class, new()
    where TGetListInput : new() { }

public abstract class AbpCrudPageBase<
    TAppService,
    TEntityDto,
    TKey,
    TGetListInput,
    TCreateInput,
    TUpdateInput
>
    : AbpCrudPageBase<
        TAppService,
        TEntityDto,
        TEntityDto,
        TKey,
        TGetListInput,
        TCreateInput,
        TUpdateInput
    >
    where TAppService : ICrudAppService<TEntityDto, TKey, TGetListInput, TCreateInput, TUpdateInput>
    where TEntityDto : IEntityDto<TKey>
    where TCreateInput : class, new()
    where TUpdateInput : class, new()
    where TGetListInput : new() { }

public abstract class AbpCrudPageBase<
    TAppService,
    TGetOutputDto,
    TGetListOutputDto,
    TKey,
    TGetListInput,
    TCreateInput,
    TUpdateInput
>
    : AbpCrudPageBase<
        TAppService,
        TGetOutputDto,
        TGetListOutputDto,
        TKey,
        TGetListInput,
        TCreateInput,
        TUpdateInput,
        TGetListOutputDto,
        TCreateInput,
        TUpdateInput
    >
    where TAppService : ICrudAppService<
            TGetOutputDto,
            TGetListOutputDto,
            TKey,
            TGetListInput,
            TCreateInput,
            TUpdateInput
        >
    where TGetOutputDto : IEntityDto<TKey>
    where TGetListOutputDto : IEntityDto<TKey>
    where TCreateInput : class, new()
    where TUpdateInput : class, new()
    where TGetListInput : new() { }

public abstract class AbpCrudPageBase<
    TAppService,
    TGetOutputDto,
    TGetListOutputDto,
    TKey,
    TGetListInput,
    TCreateInput,
    TUpdateInput,
    TListViewModel,
    TCreateViewModel,
    TUpdateViewModel
> : AbpComponentBase
    where TAppService : ICrudAppService<
            TGetOutputDto,
            TGetListOutputDto,
            TKey,
            TGetListInput,
            TCreateInput,
            TUpdateInput
        >
    where TGetOutputDto : IEntityDto<TKey>
    where TGetListOutputDto : IEntityDto<TKey>
    where TCreateInput : class, new()
    where TUpdateInput : class, new()
    where TGetListInput : new()
    where TListViewModel : IEntityDto<TKey>
    where TCreateViewModel : class, new()
    where TUpdateViewModel : class, new()
{
    [Inject]
    protected DialogService DialogService { get; set; } = default!;

    [Inject]
    protected NotificationService NotificationService { get; set; } = default!;

    [Inject]
    protected TAppService AppService { get; set; } = default!;

    [Inject]
    protected IStringLocalizer<AbpUiResource> UiLocalizer { get; set; } = default!;

    [Inject]
    public IAbpEnumLocalizer AbpEnumLocalizer { get; set; } = default!;

    protected RadzenDataGrid<TListViewModel> _grid = default!;
    protected IReadOnlyList<TListViewModel> _entities = [];
    protected int _totalCount;
    protected readonly IEnumerable<int> _pageSizeOptions = [10, 20, 30];
    protected readonly bool _showPagerSummary = true;
    protected bool _isLoading = true;
    protected string? _keyword = null;

    protected TGetListInput GetListInput = new();
    protected TCreateViewModel NewEntity;
    protected TKey EditingEntityId = default!;
    protected TUpdateViewModel EditingEntity;

    protected string? CreatePolicyName { get; set; }
    protected string? UpdatePolicyName { get; set; }
    protected string? DeletePolicyName { get; set; }

    public bool HasCreatePermission { get; set; }
    public bool HasUpdatePermission { get; set; }
    public bool HasDeletePermission { get; set; }

    protected AbpCrudPageBase()
    {
        NewEntity = new TCreateViewModel();
        EditingEntity = new TUpdateViewModel();
    }

    protected override async Task OnInitializedAsync()
    {
        await TrySetPermissionsAsync();
        await LoadDataAsync(new LoadDataArgs());
    }

    protected virtual async Task LoadDataAsync(LoadDataArgs args)
    {
        _isLoading = true;
        await UpdateGetListInputAsync(args);
        var result = await AppService.GetListAsync(GetListInput);
        _entities = MapToListViewModel(result.Items);
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
            limitedResultRequestInput.MaxResultCount = args.Top ?? 10;
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

    protected virtual async Task OpenCreateDialogAsync<TDialog>(
        string title,
        Func<DialogOptions>? func = null
    )
        where TDialog : ComponentBase
    {
        var dialogFromOption = new DialogFromOption<TCreateInput>
        {
            OnSubmit = CreateEntityAsync,
            OnCancel = CloseDialog,
            Model = await SetCreateDialogModelAsync()
        };

        bool result = await DialogService.OpenAsync<TDialog>(
            title: title,
            parameters: new Dictionary<string, object>
            {
                { "DialogFromOption", dialogFromOption },
            },
            options: func is not null
                ? func()
                : new DialogOptions()
                {
                    Draggable = true,
                    Width = "600px",
                    Height = "450px"
                }
        );

        if (result)
        {
            await _grid.Reload();
        }
    }

    protected virtual Task<TCreateInput> SetCreateDialogModelAsync()
    {
        return Task.FromResult(new TCreateInput());
    }

    protected virtual void CloseDialog()
    {
        DialogService.Close(false);
    }

    protected virtual async Task CreateEntityAsync(TCreateInput model)
    {
        try
        {
            await AppService.CreateAsync(model);
            NotificationService.Success(L["SuccessfullySaved"]);
            DialogService.Close(true);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    protected virtual async Task OpenEditDialogAsync<TDialog>(
        string title,
        TGetListOutputDto dto,
        Func<DialogOptions>? func = null
    )
        where TDialog : ComponentBase
    {
        var dialogFromOption = new DialogFromOption<TUpdateInput>
        {
            OnSubmit = UpdateEntityAsync,
            OnCancel = CloseDialog,
            Model = await SetEditDialogModelAsync(dto)
        };

        EditingEntityId = dto.Id;
        bool result = await DialogService.OpenAsync<TDialog>(
            title: title,
            parameters: new Dictionary<string, object>()
            {
                { "DialogFromOption", dialogFromOption }
            },
            options: func is not null
                ? func()
                : new DialogOptions()
                {
                    Draggable = true,
                    Width = "600px",
                    Height = "450px"
                }
        );

        if (result)
        {
            await _grid.Reload();
        }
    }

    protected abstract Task<TUpdateInput> SetEditDialogModelAsync(TGetListOutputDto dto);

    protected virtual async Task UpdateEntityAsync(TUpdateInput model)
    {
        try
        {
            await AppService.UpdateAsync(EditingEntityId, model);
            NotificationService.Success(L["SuccessfullySaved"]);
            DialogService.Close(true);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    protected virtual async Task OpenDeleteConfirmDialogAsync(
        TKey id,
        string title = "Confirm",
        string confirm = "Confirm?"
    )
    {
        var result = await DialogService.Confirm(
            message: confirm,
            title: title,
            options: new ConfirmOptions()
            {
                OkButtonText = L["Yes"],
                CancelButtonText = L["Cancel"]
            }
        );

        if (result == true)
        {
            await AppService.DeleteAsync(id);
            await _grid.Reload();
            NotificationService.Success(L["SuccessfullyDeleted"]);
        }
    }

    private IReadOnlyList<TListViewModel> MapToListViewModel(IReadOnlyList<TGetListOutputDto> dtos)
    {
        if (typeof(TGetListOutputDto) == typeof(TListViewModel))
        {
            return dtos.As<IReadOnlyList<TListViewModel>>();
        }

        return ObjectMapper.Map<IReadOnlyList<TGetListOutputDto>, List<TListViewModel>>(dtos);
    }

    protected virtual TUpdateViewModel MapToEditingEntity(TGetOutputDto entityDto)
    {
        return ObjectMapper.Map<TGetOutputDto, TUpdateViewModel>(entityDto);
    }

    protected virtual TCreateInput MapToCreateInput(TCreateViewModel createViewModel)
    {
        if (typeof(TCreateInput) == typeof(TCreateViewModel))
        {
            return createViewModel.As<TCreateInput>();
        }

        return ObjectMapper.Map<TCreateViewModel, TCreateInput>(createViewModel);
    }

    protected virtual TUpdateInput MapToUpdateInput(TUpdateViewModel updateViewModel)
    {
        if (typeof(TUpdateInput) == typeof(TUpdateViewModel))
        {
            return updateViewModel.As<TUpdateInput>();
        }

        return ObjectMapper.Map<TUpdateViewModel, TUpdateInput>(updateViewModel);
    }

    protected virtual string GetDeleteConfirmationMessage(TListViewModel entity)
    {
        return UiLocalizer["ItemWillBeDeletedMessage"];
    }

    protected virtual async Task CheckCreatePolicyAsync()
    {
        await CheckPolicyAsync(CreatePolicyName);
    }

    protected virtual async Task CheckUpdatePolicyAsync()
    {
        await CheckPolicyAsync(UpdatePolicyName);
    }

    protected virtual async Task CheckDeletePolicyAsync()
    {
        await CheckPolicyAsync(DeletePolicyName);
    }

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
