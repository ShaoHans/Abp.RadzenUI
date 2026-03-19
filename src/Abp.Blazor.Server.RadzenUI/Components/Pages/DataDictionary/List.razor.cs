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

    private RadzenDataGrid<DataDictionaryTypeDto> _typeGrid = default!;
    private IReadOnlyList<DataDictionaryTypeDto> _types = [];
    private DataDictionaryTypeDto? _selectedType;
    private bool _isTypeLoading;
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
        _isTypeLoading = true;

        try
        {
            var result = await TypeAppService.GetListAsync(new GetDataDictionaryTypesInput
            {
                Filter = _typeFilter,
                MaxResultCount = 1000,
                SkipCount = 0
            });

            _types = result.Items.ToList();
            await SyncSelectedTypeAsync();
        }
        finally
        {
            _isTypeLoading = false;
        }
    }

    private async Task SearchTypesAsync()
    {
        await LoadTypesAsync();
    }

    private async Task ResetTypesAsync()
    {
        _typeFilter = null;
        await LoadTypesAsync();
    }

    private async Task SelectTypeAsync(DataDictionaryTypeDto type)
    {
        if (_selectedType?.Id == type.Id)
        {
            return;
        }

        _selectedType = type;
        _itemFilter = null;

        if (_itemGrid is not null)
        {
            await _itemGrid.FirstPage(true);
        }
    }

    private void OnTypeRowRender(RowRenderEventArgs<DataDictionaryTypeDto> args)
    {
        if (args.Data != null && _selectedType?.Id == args.Data.Id)
        {
            args.Attributes["style"] = "background-color: var(--rz-base-200);";
        }
    }

    private async Task SyncSelectedTypeAsync()
    {
        if (_types.Count == 0)
        {
            _selectedType = null;
            ClearItems();
            return;
        }

        var matchingType = _selectedType == null
            ? _types.FirstOrDefault()
            : _types.FirstOrDefault(x => x.Id == _selectedType.Id) ?? _types.FirstOrDefault();

        if (matchingType == null)
        {
            _selectedType = null;
            ClearItems();
            return;
        }

        var typeChanged = _selectedType?.Id != matchingType.Id;
        _selectedType = matchingType;

        if (_itemGrid is not null && (typeChanged || _items.Count == 0))
        {
            await _itemGrid.FirstPage(true);
        }
    }

    private void ClearItems()
    {
        _items = [];
        _itemTotalCount = 0;
        _isItemLoading = false;
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
        if (_selectedType == null)
        {
            ClearItems();
            return;
        }

        _isItemLoading = true;

        try
        {
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
        }
        finally
        {
            _isItemLoading = false;
        }
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
