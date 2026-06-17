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

    private readonly List<WorkOrderStatus> _statuses = Enum.GetValues<WorkOrderStatus>().ToList();
    private readonly List<OperationPriority> _priorities = Enum.GetValues<OperationPriority>().ToList();
    private readonly List<WorkOrderType> _types = Enum.GetValues<WorkOrderType>().ToList();

    private RadzenDataGrid<WorkOrderDto> _grid = default!;
    private IReadOnlyList<WorkOrderDto> _items = [];
    private IList<WorkOrderDto> _selectedWorkOrders = [];
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
            _selectedWorkOrders = _selectedWorkOrders.Where(selected => _items.Any(item => item.Id == selected.Id)).ToList();
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
        await OperationAppService.AssignWorkOrdersAsync(new AssignWorkOrdersDto
        {
            WorkOrderIds = _selectedWorkOrders.Select(x => x.Id).ToList(),
            OwnerName = "调度专员"
        });
        _selectedWorkOrders = [];
        await ReloadAsync();
    }

    private bool? IsAllSelected()
    {
        if (!_items.Any() || !_selectedWorkOrders.Any())
        {
            return false;
        }

        return _selectedWorkOrders.Count == _items.Count ? true : null;
    }

    private void ToggleSelectAll(bool? selected)
    {
        _selectedWorkOrders = selected == true ? _items.ToList() : [];
    }

    private void ToggleSelection(WorkOrderDto item, bool selected)
    {
        var current = _selectedWorkOrders.ToList();
        if (selected && current.All(x => x.Id != item.Id))
        {
            current.Add(item);
        }
        else if (!selected)
        {
            current.RemoveAll(x => x.Id == item.Id);
        }

        _selectedWorkOrders = current;
    }

    public void Dispose()
    {
        _sideDialogCoordinator.Dispose();
    }
}
