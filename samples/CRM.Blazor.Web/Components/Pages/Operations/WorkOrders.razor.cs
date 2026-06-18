using Abp.RadzenUI;
using CRM.Enums;
using CRM.Localization;
using CRM.Operations;
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;
using Volo.Abp.Application.Dtos;

namespace CRM.Blazor.Web.Components.Pages.Operations;

public partial class WorkOrders : IDisposable
{
    [Inject]
    public IOperationAppService OperationAppService { get; set; } = default!;

    [Inject]
    public ISideDialogCoordinatorFactory SideDialogCoordinatorFactory { get; set; } = default!;

    [Inject]
    public DialogService DialogService { get; set; } = default!;

    private readonly List<WorkOrderStatus> _statuses = Enum.GetValues<WorkOrderStatus>().ToList();
    private readonly List<OperationPriority> _priorities = Enum.GetValues<OperationPriority>().ToList();
    private readonly List<WorkOrderType> _types = Enum.GetValues<WorkOrderType>().ToList();

    private RadzenDataGrid<WorkOrderDto> _grid = default!;
    private IReadOnlyList<WorkOrderDto> _items = [];
    private HashSet<Guid> _selectedWorkOrderIds = [];
    private int _totalCount;
    private bool _isLoading;
    private string? _filter;
    private WorkOrderStatus? _status;
    private OperationPriority? _priority;
    private WorkOrderType? _type;
    private string? _ownerName;
    private SideDialogCoordinator<WorkOrderDetailDto> _sideDialogCoordinator = default!;

    public WorkOrders()
    {
        LocalizationResource = typeof(CRMResource);
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        _sideDialogCoordinator = SideDialogCoordinatorFactory.Create<WorkOrderDetailDto>();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
        {
            await ReloadAsync();
        }
    }

    private async Task LoadDataAsync(LoadDataArgs args)
    {
        try
        {
            _isLoading = true;
            var result = await OperationAppService.GetWorkOrdersAsync(new GetWorkOrdersInput
            {
                Filter = _filter,
                Status = _status,
                Priority = _priority,
                Type = _type,
                OwnerName = _ownerName,
                SkipCount = args.Skip ?? 0,
                MaxResultCount = args.Top ?? 20,
                Sorting = args.OrderBy
            });

            _items = result.Items;
            _totalCount = (int)result.TotalCount;
            _selectedWorkOrderIds.RemoveWhere(id => _items.All(item => item.Id != id));
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
        finally
        {
            _isLoading = false;
        }
    }

    private async Task ReloadAsync()
    {
        if (_grid is not null)
        {
            await _grid.FirstPage(true);
        }
    }

    private async Task OpenDetailAsync(WorkOrderDto item)
    {
        var detail = await OperationAppService.GetWorkOrderDetailAsync(item.Id);
        await _sideDialogCoordinator.OpenDetailAsync<WorkOrderDetailDto, WorkOrderDetail>(
            detail,
            item.Code,
            "Detail",
            "620px"
        );
    }

    private async Task ChangeStatusAsync(WorkOrderDto item, RadzenSplitButtonItem args)
    {
        if (Enum.TryParse<WorkOrderStatus>(args.Value?.ToString(), out var status))
        {
            await OperationAppService.ChangeWorkOrderStatusAsync(item.Id, new ChangeWorkOrderStatusDto
            {
                Status = status
            });
            await ReloadAsync();
        }
    }

    private async Task BatchAssignAsync()
    {
        if (!_selectedWorkOrderIds.Any())
        {
            return;
        }

        var ownerName = await DialogService.OpenAsync<BatchAssignDialog>(
            L["BatchAssign"],
            new Dictionary<string, object?>
            {
                { "SelectedCount", _selectedWorkOrderIds.Count }
            },
            new DialogOptions
            {
                Draggable = true,
                Width = "420px"
            }
        ) as string;

        if (string.IsNullOrWhiteSpace(ownerName))
        {
            return;
        }

        await OperationAppService.AssignWorkOrdersAsync(new AssignWorkOrdersDto
        {
            WorkOrderIds = _selectedWorkOrderIds.ToList(),
            OwnerName = ownerName.Trim()
        });

        _selectedWorkOrderIds = [];
        await ReloadAsync();
    }

    private bool? IsAllSelected()
    {
        if (!_items.Any() || !_selectedWorkOrderIds.Any())
        {
            return false;
        }

        return _items.All(item => _selectedWorkOrderIds.Contains(item.Id)) ? true : null;
    }

    private void ToggleSelectAll(bool? selected)
    {
        _selectedWorkOrderIds = selected == true
            ? _items.Select(x => x.Id).ToHashSet()
            : [];
    }

    private bool IsSelected(WorkOrderDto item)
    {
        return _selectedWorkOrderIds.Contains(item.Id);
    }

    private void ToggleSelection(WorkOrderDto item, bool selected)
    {
        if (selected)
        {
            _selectedWorkOrderIds.Add(item.Id);
        }
        else
        {
            _selectedWorkOrderIds.Remove(item.Id);
        }
    }

    public void Dispose()
    {
        _sideDialogCoordinator.Dispose();
    }
}
