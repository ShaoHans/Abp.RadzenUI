using Abp.RadzenUI.Application.Contracts.DataDictionaries;
using Abp.RadzenUI.Localization;
using Abp.RadzenUI.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
using Radzen;
using Radzen.Blazor;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.ExceptionHandling;
using Volo.Abp.AspNetCore.Components.Notifications;

namespace Abp.RadzenUI.Components.Pages.DataDictionary;

public partial class List
{
    [Microsoft.AspNetCore.Components.Inject]
    protected IDataDictionaryTypeAppService TypeAppService { get; set; } = default!;

    [Microsoft.AspNetCore.Components.Inject]
    protected IDataDictionaryItemAppService ItemAppService { get; set; } = default!;

    [Microsoft.AspNetCore.Components.Inject]
    protected DialogService DialogService { get; set; } = default!;

    [Microsoft.AspNetCore.Components.Inject]
    protected IUiNotificationService Notify { get; set; } = default!;

    [Microsoft.AspNetCore.Components.Inject]
    protected IUserExceptionInformer ExceptionInformer { get; set; } = default!;

    [Microsoft.AspNetCore.Components.Inject]
    public IStringLocalizer<AbpRadzenUIResource> UL { get; set; } = default!;

    private List<DataDictionaryTypeDto> _types = [];
    private DataDictionaryTypeDto? _selectedType;
    private string? _typeFilter;

    private RadzenDataGrid<DataDictionaryItemDto> _itemGrid = default!;
    private IReadOnlyList<DataDictionaryItemDto> _items = [];
    private int _itemTotalCount;
    private bool _isItemLoading;
    private string? _itemFilter;
    private readonly int _defaultPageSize = 10;
    private readonly IEnumerable<int> _pageSizeOptions = [10, 20, 30];

    public bool HasCreatePermission { get; set; }
    public bool HasUpdatePermission { get; set; }
    public bool HasDeletePermission { get; set; }

    public List()
    {
        LocalizationResource = typeof(AbpRadzenUIResource);
    }

    protected override async Task OnInitializedAsync()
    {
        await SetPermissionsAsync();
        await LoadTypesAsync();
    }

    private async Task SetPermissionsAsync()
    {
        HasCreatePermission = await AuthorizationService.IsGrantedAsync(RadzenUIPermissions.DataDictionary.Create);
        HasUpdatePermission = await AuthorizationService.IsGrantedAsync(RadzenUIPermissions.DataDictionary.Update);
        HasDeletePermission = await AuthorizationService.IsGrantedAsync(RadzenUIPermissions.DataDictionary.Delete);
    }

    private async Task LoadTypesAsync()
    {
        var result = await TypeAppService.GetListAsync(new GetDataDictionaryTypesInput
        {
            Filter = _typeFilter,
            MaxResultCount = 1000,
            SkipCount = 0
        });
        _types = result.Items.ToList();
    }

    private void SelectType(DataDictionaryTypeDto type)
    {
        _selectedType = type;
        _itemFilter = null;
        _itemGrid?.Reload();
    }

    private string GetTypeCardStyle(DataDictionaryTypeDto type)
    {
        return _selectedType?.Id == type.Id
            ? "cursor:pointer; border-left:3px solid var(--rz-primary)"
            : "cursor:pointer";
    }

    #region Type CRUD

    private async Task OpenCreateTypeDialogAsync()
    {
        var dialogFromOption = new Models.DialogFromOption<CreateDataDictionaryTypeDto>
        {
            Model = new CreateDataDictionaryTypeDto(),
            OnSubmit = async model =>
            {
                try
                {
                    await TypeAppService.CreateAsync(model);
                    await Notify.Success(UL["SavedSuccessfully"]);
                    DialogService.Close(true);
                }
                catch (Exception ex)
                {
                    await ExceptionInformer.InformAsync(new UserExceptionInformerContext(ex));
                }
            },
            OnCancel = () => DialogService.Close(false)
        };

        var parameters = new Dictionary<string, object>
        {
            { "DialogFromOption", dialogFromOption }
        };

        var result = await DialogService.OpenAsync<CreateType>(
            L["DataDictionary:CreateType"],
            parameters,
            new DialogOptions { Draggable = true, Width = "600px" });

        if (result is true)
        {
            await LoadTypesAsync();
        }
    }

    private async Task OpenEditTypeDialogAsync(DataDictionaryTypeDto type)
    {
        var dialogFromOption = new Models.DialogFromOption<UpdateDataDictionaryTypeDto>
        {
            Model = new UpdateDataDictionaryTypeDto
            {
                Name = type.Name,
                Description = type.Description
            },
            OnSubmit = async model =>
            {
                try
                {
                    await TypeAppService.UpdateAsync(type.Id, model);
                    await Notify.Success(UL["SavedSuccessfully"]);
                    DialogService.Close(true);
                }
                catch (Exception ex)
                {
                    await ExceptionInformer.InformAsync(new UserExceptionInformerContext(ex));
                }
            },
            OnCancel = () => DialogService.Close(false)
        };

        var parameters = new Dictionary<string, object>
        {
            { "DialogFromOption", dialogFromOption },
            { "Code", type.Code }
        };

        var result = await DialogService.OpenAsync<EditType>(
            L["DataDictionary:EditType"],
            parameters,
            new DialogOptions { Draggable = true, Width = "600px" });

        if (result is true)
        {
            await LoadTypesAsync();
            if (_selectedType?.Id == type.Id)
            {
                _selectedType = _types.Find(t => t.Id == type.Id);
            }
        }
    }

    private async Task DeleteTypeAsync(DataDictionaryTypeDto type)
    {
        var confirmed = await DialogService.Confirm(
            L["DataDictionary:DeleteType.Confirm", type.Name],
            UL["AreYouSure"],
            new ConfirmOptions { OkButtonText = UL["Yes"], CancelButtonText = UL["Cancel"] });

        if (confirmed == true)
        {
            try
            {
                await TypeAppService.DeleteAsync(type.Id);
                await Notify.Success(UL["DeletedSuccessfully"]);
                if (_selectedType?.Id == type.Id)
                {
                    _selectedType = null;
                }
                await LoadTypesAsync();
            }
            catch (Exception ex)
            {
                await ExceptionInformer.InformAsync(new UserExceptionInformerContext(ex));
            }
        }
    }

    #endregion

    #region Item CRUD

    private async Task LoadItemsAsync(LoadDataArgs args)
    {
        if (_selectedType == null) return;

        _isItemLoading = true;

        var input = new GetDataDictionaryItemsInput
        {
            DataDictionaryTypeId = _selectedType.Id,
            Filter = _itemFilter,
            Sorting = args.OrderBy,
            SkipCount = args.Skip ?? 0,
            MaxResultCount = args.Top ?? _defaultPageSize
        };

        var result = await ItemAppService.GetListAsync(input);
        _items = result.Items;
        _itemTotalCount = (int)result.TotalCount;
        _isItemLoading = false;
    }

    private async Task SearchItemsAsync()
    {
        if (_itemGrid is not null)
        {
            await _itemGrid.Reload();
        }
    }

    private async Task ResetItemsAsync()
    {
        _itemFilter = null;
        await SearchItemsAsync();
    }

    private async Task OpenCreateItemDialogAsync()
    {
        if (_selectedType == null) return;

        var dialogFromOption = new Models.DialogFromOption<CreateDataDictionaryItemDto>
        {
            Model = new CreateDataDictionaryItemDto
            {
                DataDictionaryTypeId = _selectedType.Id,
                Sort = 0
            },
            OnSubmit = async model =>
            {
                try
                {
                    await ItemAppService.CreateAsync(model);
                    await Notify.Success(UL["SavedSuccessfully"]);
                    DialogService.Close(true);
                }
                catch (Exception ex)
                {
                    await ExceptionInformer.InformAsync(new UserExceptionInformerContext(ex));
                }
            },
            OnCancel = () => DialogService.Close(false)
        };

        var parameters = new Dictionary<string, object>
        {
            { "DialogFromOption", dialogFromOption }
        };

        var result = await DialogService.OpenAsync<CreateItem>(
            L["DataDictionary:CreateItem"],
            parameters,
            new DialogOptions { Draggable = true, Width = "600px" });

        if (result is true)
        {
            await _itemGrid.Reload();
        }
    }

    private async Task OpenEditItemDialogAsync(DataDictionaryItemDto item)
    {
        var dialogFromOption = new Models.DialogFromOption<UpdateDataDictionaryItemDto>
        {
            Model = new UpdateDataDictionaryItemDto
            {
                Name = item.Name,
                Sort = item.Sort,
                IsActive = item.IsActive,
                Description = item.Description
            },
            OnSubmit = async model =>
            {
                try
                {
                    await ItemAppService.UpdateAsync(item.Id, model);
                    await Notify.Success(UL["SavedSuccessfully"]);
                    DialogService.Close(true);
                }
                catch (Exception ex)
                {
                    await ExceptionInformer.InformAsync(new UserExceptionInformerContext(ex));
                }
            },
            OnCancel = () => DialogService.Close(false)
        };

        var parameters = new Dictionary<string, object>
        {
            { "DialogFromOption", dialogFromOption },
            { "Code", item.Code }
        };

        var result = await DialogService.OpenAsync<EditItem>(
            L["DataDictionary:EditItem"],
            parameters,
            new DialogOptions { Draggable = true, Width = "600px" });

        if (result is true)
        {
            await _itemGrid.Reload();
        }
    }

    private async Task DeleteItemAsync(DataDictionaryItemDto item)
    {
        var confirmed = await DialogService.Confirm(
            L["DataDictionary:DeleteItem.Confirm", item.Name],
            UL["AreYouSure"],
            new ConfirmOptions { OkButtonText = UL["Yes"], CancelButtonText = UL["Cancel"] });

        if (confirmed == true)
        {
            try
            {
                await ItemAppService.DeleteAsync(item.Id);
                await Notify.Success(UL["DeletedSuccessfully"]);
                await _itemGrid.Reload();
            }
            catch (Exception ex)
            {
                await ExceptionInformer.InformAsync(new UserExceptionInformerContext(ex));
            }
        }
    }

    private async Task ToggleItemActiveAsync(DataDictionaryItemDto item)
    {
        try
        {
            await ItemAppService.ToggleActiveAsync(item.Id);
            await _itemGrid.Reload();
        }
        catch (Exception ex)
        {
            await ExceptionInformer.InformAsync(new UserExceptionInformerContext(ex));
        }
    }

    #endregion
}
