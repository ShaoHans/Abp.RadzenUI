using Abp.RadzenUI.Components.Shared;
using Abp.RadzenUI.Features.Export;
using Abp.RadzenUI.Localization;
using Abp.RadzenUI.Models;
using Abp.RadzenUI.Infrastructure.Services;
using Abp.RadzenUI.Infrastructure.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;
using System.Globalization;
using System.Linq;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.AspNetCore.Components;
using Volo.Abp.AspNetCore.Components.ExceptionHandling;
using Volo.Abp.Authorization;
using Volo.Abp.Localization;

namespace Abp.RadzenUI;

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
> : AbpRadzenUIComponentBase
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
    protected TAppService AppService { get; set; } = default!;

    [Inject]
    public IAbpEnumLocalizer AbpEnumLocalizer { get; set; } = default!;

    [Inject]
    public IStringLocalizer<AbpRadzenUIResource> UL { get; set; } = default!;

    [Inject]
    protected IHttpContextAccessor HttpContextAccessor { get; set; } = default!;

    [Inject]
    protected IJSRuntime JSRuntime { get; set; } = default!;

    [Inject]
    protected GridPageSizePreferenceService GridPageSizePreferenceService { get; set; } =
        default!;

    [Inject]
    protected IDataExportManager ExportManager { get; set; } = default!;

    protected RadzenDataGrid<TListViewModel> _grid = default!;
    protected IReadOnlyList<TListViewModel> _entities = [];
    protected int _totalCount;
    protected readonly IEnumerable<int> _pageSizeOptions = [10, 20, 30, 50, 100];
    protected readonly bool _showPagerSummary = true;
    protected bool _isLoading = true;
    protected int _defaultPageSize = 10;
    bool _isInteractive;
    int? _lastPersistedPageSize;

    protected TGetListInput GetListInput = new();
    protected TCreateViewModel NewEntity;
    protected TKey EditingEntityId = default!;
    protected TUpdateViewModel EditingEntity;

    protected string? CreatePolicyName { get; set; }
    protected string? UpdatePolicyName { get; set; }
    protected string? DeletePolicyName { get; set; }
    protected string? ExportPolicyName { get; set; }

    public bool HasCreatePermission { get; set; }
    public bool HasUpdatePermission { get; set; }
    public bool HasDeletePermission { get; set; }

    /// <summary>
    /// Whether the current user may export. Resolved in <see cref="SetPermissionsAsync"/>:
    /// <c>true</c> when <see cref="ExportPolicyName"/> is not set (the page-level authorization
    /// already gates access), otherwise the result of the policy check.
    /// </summary>
    public bool HasExportPermission { get; set; } = true;

    /// <summary>Set while an export is running so the toolbar button can show a busy state.</summary>
    protected bool IsExporting { get; private set; }

    protected AbpCrudPageBase()
    {
        NewEntity = new TCreateViewModel();
        EditingEntity = new TUpdateViewModel();
    }

    protected override async Task OnInitializedAsync()
    {
        await TrySetPermissionsAsync();
        InitializePageSizePreference();
        await LoadDataAsync(new LoadDataArgs { Top = _defaultPageSize });
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
        {
            return;
        }

        _isInteractive = true;
        await PersistPageSizeCookieAsync(_defaultPageSize);
    }

    protected virtual async Task LoadDataAsync(LoadDataArgs args)
    {
        _isLoading = true;
        await UpdatePageSizePreferenceAsync(args);
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
            limitedResultRequestInput.MaxResultCount = args.Top ?? _defaultPageSize;
        }

        return Task.CompletedTask;
    }

    void InitializePageSizePreference()
    {
        var pageSize =
            GridPageSizePreferenceService.PageSize
            ?? AbpRadzenUICookieHelper.GetPageSizeCookie(HttpContextAccessor.HttpContext);

        if (!IsSupportedPageSize(pageSize))
        {
            return;
        }

        _defaultPageSize = pageSize!.Value;
        GridPageSizePreferenceService.PageSize = _defaultPageSize;
        _lastPersistedPageSize = _defaultPageSize;
    }

    async Task UpdatePageSizePreferenceAsync(LoadDataArgs args)
    {
        if (!IsSupportedPageSize(args.Top))
        {
            return;
        }

        _defaultPageSize = args.Top!.Value;
        GridPageSizePreferenceService.PageSize = _defaultPageSize;

        if (_isInteractive)
        {
            await PersistPageSizeCookieAsync(_defaultPageSize);
        }
    }

    bool IsSupportedPageSize(int? pageSize)
    {
        return pageSize.HasValue && _pageSizeOptions.Contains(pageSize.Value);
    }

    async Task PersistPageSizeCookieAsync(int pageSize)
    {
        if (_lastPersistedPageSize == pageSize)
        {
            return;
        }

        await JSRuntime.InvokeVoidAsync(
            "abpRadzenCookie.set",
            AbpRadzenUICookieHelper.PageSizeKey,
            pageSize.ToString(CultureInfo.InvariantCulture),
            3650
        );

        _lastPersistedPageSize = pageSize;
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

        HasExportPermission =
            ExportPolicyName == null
            || await AuthorizationService.IsGrantedAsync(ExportPolicyName);
    }

    protected virtual async Task OpenCreateDialogAsync<TDialog>(
        string title,
        Func<DialogOptions>? func = null,
        Dictionary<string, object?>? parameters = null,
        Func<Task>? callback = null
    )
        where TDialog : ComponentBase
    {
        parameters ??= [];
        var dialogFromOption = new DialogFromOption<TCreateInput>
        {
            OnSubmit = CreateEntityAsync,
            OnCancel = CloseDialog,
            Model = await SetCreateDialogModelAsync(),
        };
        parameters.Add("DialogFromOption", dialogFromOption);

        bool? result = await DialogService.OpenAsync<TDialog>(
            title: title,
            parameters: parameters,
            options: func is not null
                ? func()
                : new DialogOptions() { Draggable = true, Width = "600px" }
        );

        if (result == true)
        {
            if (_grid != null)
            {
                await _grid.Reload();
            }
            if (callback != null)
            {
                await callback();
            }
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
            await Notify.Success(UL["SavedSuccessfully"]);
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
        Func<DialogOptions>? func = null,
        Dictionary<string, object?>? parameters = null,
        Func<Task>? callback = null
    )
        where TDialog : ComponentBase
    {
        parameters ??= [];
        var dialogFromOption = new DialogFromOption<TUpdateInput>
        {
            OnSubmit = UpdateEntityAsync,
            OnCancel = CloseDialog,
            Model = await SetEditDialogModelAsync(dto),
        };
        parameters.Add("DialogFromOption", dialogFromOption);

        EditingEntityId = dto.Id;
        bool? result = await DialogService.OpenAsync<TDialog>(
            title: title,
            parameters: parameters,
            options: func is not null
                ? func()
                : new DialogOptions() { Draggable = true, Width = "600px" }
        );

        if (result == true)
        {
            if (_grid != null)
            {
                await _grid.Reload();
            }
            if (callback != null)
            {
                await callback();
            }
        }
    }

    protected abstract Task<TUpdateInput> SetEditDialogModelAsync(TGetListOutputDto dto);

    protected virtual async Task UpdateEntityAsync(TUpdateInput model)
    {
        try
        {
            await AppService.UpdateAsync(EditingEntityId, model);
            await Notify.Success(UL["SavedSuccessfully"]);
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
        string confirm = "Confirm?",
        Func<Task>? callback = null
    )
    {
        var result = await DialogService.Confirm(
            message: confirm,
            title: title,
            options: new ConfirmOptions()
            {
                OkButtonText = UL["Yes"],
                CancelButtonText = UL["Cancel"],
            }
        );

        if (result == true)
        {
            try
            {
                await AppService.DeleteAsync(id);
                if (_grid != null)
                {
                    await _grid.Reload();
                }
                if (callback != null)
                {
                    await callback();
                }
                await Notify.Success(UL["DeletedSuccessfully"]);
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
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

    #region Export

    /// <summary>Overall safety cap on the number of rows exported. Override to change.</summary>
    protected virtual int ExportMaxCount => 100000;

    /// <summary>Rows fetched per page while streaming the export. Bounds peak memory. Override to change.</summary>
    protected virtual int ExportPageSize => 1000;

    /// <summary>Worksheet name used in the generated file. Override to localize/customize.</summary>
    protected virtual string ExportSheetName => typeof(TGetListOutputDto).Name;

    /// <summary>
    /// Convenience entry point wired to the toolbar export button. It is only a thin adapter: it
    /// manages the busy state / error surface and hands a <see cref="ExcelExportOptions{T}"/> built
    /// from the overridable members below to the reusable <see cref="IDataExportManager"/>, which
    /// owns the actual flow (permission → gate → fetch → shape → serialize → download → notify).
    /// <para>
    /// A page that does <b>not</b> derive from this base does not need any of this — it can inject
    /// <see cref="IDataExportManager"/> and call it directly with its own <c>DataProvider</c>.
    /// </para>
    /// Override this whole method only for fully custom flows; for the common "verify before export"
    /// case override <see cref="OnBeforeExportAsync"/> instead.
    /// </summary>
    public virtual async Task ExportAsync()
    {
        if (IsExporting)
        {
            return;
        }

        IsExporting = true;
        StateHasChanged();
        try
        {
            await ExportManager.ExportToExcelAsync(
                new ExcelExportOptions<TGetListOutputDto>
                {
                    PolicyName = ExportPolicyName,
                    BeforeExportAsync = OnBeforeExportAsync,
                    PageDataProvider = (skipCount, maxResultCount, _) =>
                        GetExportPageAsync(skipCount, maxResultCount),
                    RowSelector = MapToExportRows,
                    FileName = GetExportFileName(),
                    SheetName = ExportSheetName,
                    PageSize = ExportPageSize,
                    MaxCount = ExportMaxCount,
                }
            );
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
        finally
        {
            IsExporting = false;
            StateHasChanged();
        }
    }

    /// <summary>
    /// Runs before any data is fetched. Return <c>false</c> to abort the export.
    /// This is the primary extension point for scenarios that must be authorized interactively —
    /// e.g. open a verification-code dialog and only return <c>true</c> once it is verified:
    /// <code>
    /// protected override async Task&lt;bool&gt; OnBeforeExportAsync()
    /// {
    ///     var ok = await DialogService.OpenAsync&lt;CaptchaDialog&gt;("Verify");
    ///     return ok == true;
    /// }
    /// </code>
    /// Default returns <c>true</c> (no extra gate).
    /// </summary>
    protected virtual Task<bool> OnBeforeExportAsync() => Task.FromResult(true);

    /// <summary>
    /// Fetches a single page of rows for the export. Called repeatedly with an advancing
    /// <paramref name="skipCount"/> until a short/empty page is returned, so the whole result set is
    /// never held in memory at once. By default reuses the current <see cref="GetListInput"/> (the
    /// active filter/sort are preserved) and only adjusts the paging window. Override to customize
    /// the query.
    /// </summary>
    protected virtual async Task<IReadOnlyList<TGetListOutputDto>> GetExportPageAsync(
        int skipCount,
        int maxResultCount
    )
    {
        if (GetListInput is IPagedResultRequest pagedResultRequestInput)
        {
            pagedResultRequestInput.SkipCount = skipCount;
        }

        if (GetListInput is ILimitedResultRequest limitedResultRequestInput)
        {
            limitedResultRequestInput.MaxResultCount = maxResultCount;
        }

        var result = await AppService.GetListAsync(GetListInput);
        return result.Items;
    }

    /// <summary>
    /// Shapes the fetched rows into the object passed to <see cref="IExcelExporter"/>.
    /// Default returns the DTOs as-is, so the exported column headers are the DTO property names.
    /// Override and return a list of <c>Dictionary&lt;string, object?&gt;</c> to emit localized
    /// headers and a curated column set.
    /// </summary>
    protected virtual object MapToExportRows(IReadOnlyList<TGetListOutputDto> data) => data;

    /// <summary>Builds the download file name. Default: <c>{Entity}-{yyyyMMddHHmmss}.xlsx</c>.</summary>
    protected virtual string GetExportFileName()
    {
        var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);
        return $"{typeof(TGetListOutputDto).Name}-{timestamp}.xlsx";
    }

    #endregion
}
