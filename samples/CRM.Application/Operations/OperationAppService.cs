using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using CRM.Enums;
using CRM.Permissions;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace CRM.Operations;

public class OperationAppService(
    IRepository<WorkOrder, Guid> workOrderRepository,
    IRepository<WorkOrderEvent, Guid> eventRepository,
    IRepository<OperationAsset, Guid> assetRepository,
    IRepository<OperationShift, Guid> shiftRepository
) : ApplicationService, IOperationAppService
{
    public async Task<OperationDashboardDto> GetDashboardAsync()
    {
        await CheckPolicyAsync(CRMPermissions.Operations.Dashboard);

        var workOrders = await workOrderRepository.GetListAsync();
        var assets = await assetRepository.GetListAsync();
        var today = Clock.Now.Date;

        return new OperationDashboardDto
        {
            TotalWorkOrders = workOrders.Count,
            OpenWorkOrders = workOrders.Count(x => x.Status is not WorkOrderStatus.Resolved and not WorkOrderStatus.Closed),
            CriticalWorkOrders = workOrders.Count(x => x.Priority == OperationPriority.Critical),
            OnlineAssets = assets.Count(x => x.Status == AssetStatus.Online),
            AverageHealthScore = assets.Count == 0 ? 0 : Math.Round((decimal)assets.Average(x => x.HealthScore), 1),
            WorkOrdersByStatus = Enum.GetValues<WorkOrderStatus>()
                .Select(status => new OperationMetricDto
                {
                    Name = status.ToString(),
                    Value = workOrders.Count(x => x.Status == status),
                    Color = GetStatusColor(status)
                })
                .ToList(),
            WorkOrdersByPriority = Enum.GetValues<OperationPriority>()
                .Select(priority => new OperationMetricDto
                {
                    Name = priority.ToString(),
                    Value = workOrders.Count(x => x.Priority == priority),
                    Color = GetPriorityColor(priority)
                })
                .ToList(),
            AssetHealthBands =
            [
                new() { Name = "90+", Value = assets.Count(x => x.HealthScore >= 90), Color = "#16a34a" },
                new() { Name = "75-89", Value = assets.Count(x => x.HealthScore is >= 75 and < 90), Color = "#0ea5e9" },
                new() { Name = "60-74", Value = assets.Count(x => x.HealthScore is >= 60 and < 75), Color = "#f59e0b" },
                new() { Name = "<60", Value = assets.Count(x => x.HealthScore < 60), Color = "#dc2626" }
            ],
            LastSevenDays = Enumerable.Range(0, 7)
                .Select(offset => today.AddDays(offset - 6))
                .Select(date => new OperationMetricDto
                {
                    Name = date.ToString("MM-dd"),
                    Value = workOrders.Count(x => x.CreationTime.Date == date),
                    Color = "#2563eb"
                })
                .ToList()
        };
    }

    public async Task<PagedResultDto<WorkOrderDto>> GetWorkOrdersAsync(GetWorkOrdersInput input)
    {
        await CheckPolicyAsync(CRMPermissions.Operations.WorkOrders);

        var query = await workOrderRepository.GetQueryableAsync();
        query = ApplyWorkOrderFilter(query, input);
        var totalCount = query.Count();

        query = input.Sorting.IsNullOrWhiteSpace()
            ? query.OrderByDescending(x => x.PlannedAt)
            : query.OrderBy(input.Sorting);

        var items = query
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount)
            .ToList();

        return new PagedResultDto<WorkOrderDto>(
            totalCount,
            ObjectMapper.Map<List<WorkOrder>, List<WorkOrderDto>>(items)
        );
    }

    public async Task<WorkOrderDetailDto> GetWorkOrderDetailAsync(Guid id)
    {
        await CheckPolicyAsync(CRMPermissions.Operations.WorkOrders);

        var workOrder = await workOrderRepository.GetAsync(id);
        var events = (await eventRepository.GetListAsync(x => x.WorkOrderId == id))
            .OrderByDescending(x => x.EventTime)
            .ToList();

        return new WorkOrderDetailDto
        {
            WorkOrder = ObjectMapper.Map<WorkOrder, WorkOrderDto>(workOrder),
            Events = ObjectMapper.Map<List<WorkOrderEvent>, List<WorkOrderEventDto>>(events)
        };
    }

    public async Task ChangeWorkOrderStatusAsync(Guid id, ChangeWorkOrderStatusDto input)
    {
        await CheckPolicyAsync(CRMPermissions.Operations.WorkOrderActions);

        var workOrder = await workOrderRepository.GetAsync(id);
        workOrder.Status = input.Status;
        workOrder.CompletedAt = input.Status is WorkOrderStatus.Resolved or WorkOrderStatus.Closed
            ? Clock.Now
            : null;

        await workOrderRepository.UpdateAsync(workOrder);
        await eventRepository.InsertAsync(new WorkOrderEvent
        {
            WorkOrderId = id,
            Status = input.Status,
            OperatorName = CurrentUser.UserName ?? "admin",
            Summary = input.Summary.IsNullOrWhiteSpace() ? $"状态更新为 {input.Status}" : input.Summary!,
            EventTime = Clock.Now
        });
    }

    public async Task AssignWorkOrdersAsync(AssignWorkOrdersDto input)
    {
        await CheckPolicyAsync(CRMPermissions.Operations.WorkOrderActions);

        if (input.OwnerName.IsNullOrWhiteSpace())
        {
            throw new BusinessException("CRM:Operations:OwnerRequired");
        }

        foreach (var id in input.WorkOrderIds.Distinct())
        {
            var workOrder = await workOrderRepository.GetAsync(id);
            workOrder.OwnerName = input.OwnerName;
            workOrder.Status = WorkOrderStatus.Assigned;
            await workOrderRepository.UpdateAsync(workOrder);

            await eventRepository.InsertAsync(new WorkOrderEvent
            {
                WorkOrderId = id,
                Status = WorkOrderStatus.Assigned,
                OperatorName = CurrentUser.UserName ?? "admin",
                Summary = $"分派给 {input.OwnerName}",
                EventTime = Clock.Now
            });
        }
    }

    public async Task<PagedResultDto<OperationAssetDto>> GetAssetsAsync(GetAssetsInput input)
    {
        await CheckPolicyAsync(CRMPermissions.Operations.Assets);

        var query = await assetRepository.GetQueryableAsync();

        if (!input.Filter.IsNullOrWhiteSpace())
        {
            query = query.Where(x =>
                x.Code.Contains(input.Filter!) ||
                x.Name.Contains(input.Filter!) ||
                x.Location.Contains(input.Filter!)
            );
        }

        if (input.Status.HasValue)
        {
            query = query.Where(x => x.Status == input.Status.Value);
        }

        if (!input.Category.IsNullOrWhiteSpace())
        {
            query = query.Where(x => x.Category == input.Category);
        }

        var totalCount = query.Count();
        query = input.Sorting.IsNullOrWhiteSpace()
            ? query.OrderBy(x => x.HealthScore)
            : query.OrderBy(input.Sorting);

        var items = query
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount)
            .ToList();

        return new PagedResultDto<OperationAssetDto>(
            totalCount,
            ObjectMapper.Map<List<OperationAsset>, List<OperationAssetDto>>(items)
        );
    }

    public async Task<List<OperationShiftDto>> GetShiftsAsync(DateTime start, DateTime end)
    {
        await CheckPolicyAsync(CRMPermissions.Operations.Shifts);

        var shifts = await shiftRepository.GetListAsync(x => x.ShiftDate >= start.Date && x.ShiftDate <= end.Date);
        return ObjectMapper.Map<List<OperationShift>, List<OperationShiftDto>>(
            shifts.OrderBy(x => x.ShiftDate).ThenBy(x => x.ShiftType).ToList()
        );
    }

    private static IQueryable<WorkOrder> ApplyWorkOrderFilter(IQueryable<WorkOrder> query, GetWorkOrdersInput input)
    {
        if (!input.Filter.IsNullOrWhiteSpace())
        {
            query = query.Where(x =>
                x.Code.Contains(input.Filter!) ||
                x.Title.Contains(input.Filter!) ||
                x.OwnerName.Contains(input.Filter!) ||
                x.Location.Contains(input.Filter!)
            );
        }

        if (input.Status.HasValue)
        {
            query = query.Where(x => x.Status == input.Status.Value);
        }

        if (input.Type.HasValue)
        {
            query = query.Where(x => x.Type == input.Type.Value);
        }

        if (input.Priority.HasValue)
        {
            query = query.Where(x => x.Priority == input.Priority.Value);
        }

        if (!input.OwnerName.IsNullOrWhiteSpace())
        {
            query = query.Where(x => x.OwnerName == input.OwnerName);
        }

        if (input.PlannedStart.HasValue)
        {
            query = query.Where(x => x.PlannedAt >= input.PlannedStart.Value.Date);
        }

        if (input.PlannedEnd.HasValue)
        {
            query = query.Where(x => x.PlannedAt < input.PlannedEnd.Value.Date.AddDays(1));
        }

        return query;
    }

    private static string GetStatusColor(WorkOrderStatus status)
    {
        return status switch
        {
            WorkOrderStatus.New => "#64748b",
            WorkOrderStatus.Assigned => "#2563eb",
            WorkOrderStatus.InProgress => "#7c3aed",
            WorkOrderStatus.Paused => "#f59e0b",
            WorkOrderStatus.Resolved => "#16a34a",
            WorkOrderStatus.Closed => "#0f766e",
            _ => "#64748b"
        };
    }

    private static string GetPriorityColor(OperationPriority priority)
    {
        return priority switch
        {
            OperationPriority.Low => "#64748b",
            OperationPriority.Normal => "#2563eb",
            OperationPriority.High => "#f59e0b",
            OperationPriority.Critical => "#dc2626",
            _ => "#64748b"
        };
    }
}
