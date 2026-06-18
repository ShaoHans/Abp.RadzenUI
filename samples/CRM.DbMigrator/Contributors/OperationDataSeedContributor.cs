using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bogus;
using CRM.Enums;
using CRM.Operations;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace CRM.DbMigrator.Contributors;

internal class OperationDataSeedContributor(
    IRepository<WorkOrder, Guid> workOrderRepository,
    IRepository<WorkOrderEvent, Guid> eventRepository,
    IRepository<OperationAsset, Guid> assetRepository,
    IRepository<OperationShift, Guid> shiftRepository
) : IDataSeedContributor, ITransientDependency
{
    private static readonly string[] Owners = ["张三", "李四", "王五", "欧阳修", "陈明", "赵琳"];
    private static readonly string[] Locations = ["A 区冷站", "B2 配电间", "南门客户中心", "3 号仓", "云边机房", "北区泵房"];
    private static readonly string[] AssetCategories = ["网关", "传感器", "电梯", "冷机", "摄像头", "门禁"];
    private static readonly string[] EnglishOwners = ["Olivia Chen", "Ethan Miller", "Maya Patel", "Noah Wilson", "Ava Johnson", "Liam Davis"];
    private static readonly string[] EnglishLocations = ["North Campus Plant", "Tower B Electrical Room", "Customer Success Hub", "Warehouse 3", "Edge Data Room", "West Pump Station"];
    private static readonly string[] EnglishAssetCategories = ["Gateway", "Sensor", "Elevator", "Chiller", "Camera", "Access Control"];

    public async Task SeedAsync(DataSeedContext context)
    {
        if (await workOrderRepository.GetCountAsync() == 0)
        {
            var workOrders = GetWorkOrders();
            foreach (var workOrder in workOrders)
            {
                await workOrderRepository.InsertAsync(workOrder);
                foreach (var item in GetEvents(workOrder))
                {
                    await eventRepository.InsertAsync(item);
                }
            }

            foreach (var asset in GetAssets())
            {
                await assetRepository.InsertAsync(asset);
            }

            foreach (var shift in GetShifts())
            {
                await shiftRepository.InsertAsync(shift);
            }
        }

        if (!await (await workOrderRepository.GetQueryableAsync()).AnyAsync(x => x.Code.StartsWith("EN-WO-")))
        {
            var workOrders = GetEnglishWorkOrders();
            foreach (var workOrder in workOrders)
            {
                await workOrderRepository.InsertAsync(workOrder);
                foreach (var item in GetEnglishEvents(workOrder))
                {
                    await eventRepository.InsertAsync(item);
                }
            }
        }

        if (!await (await assetRepository.GetQueryableAsync()).AnyAsync(x => x.Code.StartsWith("EN-AST-")))
        {
            foreach (var asset in GetEnglishAssets())
            {
                await assetRepository.InsertAsync(asset);
            }
        }

        if (!await (await shiftRepository.GetQueryableAsync()).AnyAsync(x => x.OwnerName == EnglishOwners[0]))
        {
            foreach (var shift in GetEnglishShifts())
            {
                await shiftRepository.InsertAsync(shift);
            }
        }
    }

    private static List<WorkOrder> GetWorkOrders()
    {
        var sequence = 1;
        var now = DateTime.Now;

        return new Faker<WorkOrder>("zh_CN")
            .UseSeed(20260617)
            .RuleFor(x => x.Code, _ => $"WO-{sequence++.ToString().PadLeft(5, '0')}")
            .RuleFor(x => x.Title, f => f.PickRandom("设备巡检", "温控异常", "客户现场支持", "安全隐患复核", "资产保养") + " - " + f.Commerce.ProductName())
            .RuleFor(x => x.Type, f => f.PickRandom<WorkOrderType>())
            .RuleFor(x => x.Status, f => f.PickRandom<WorkOrderStatus>())
            .RuleFor(x => x.Priority, f => f.PickRandom<OperationPriority>())
            .RuleFor(x => x.OwnerName, f => f.PickRandom(Owners))
            .RuleFor(x => x.Location, f => f.PickRandom(Locations))
            .RuleFor(x => x.PlannedAt, f => now.Date.AddDays(f.Random.Int(-5, 8)).AddHours(f.Random.Int(8, 20)))
            .RuleFor(x => x.CompletedAt, (_, x) => x.Status is WorkOrderStatus.Resolved or WorkOrderStatus.Closed ? x.PlannedAt.AddHours(2) : null)
            .RuleFor(x => x.Description, f => f.Lorem.Sentence(18))
            .RuleFor(x => x.CreationTime, f => now.AddDays(-f.Random.Int(0, 14)).AddMinutes(-f.Random.Int(0, 600)))
            .Generate(96);
    }

    private static List<WorkOrderEvent> GetEvents(WorkOrder workOrder)
    {
        var items = new List<WorkOrderEvent>
        {
            new()
            {
                WorkOrderId = workOrder.Id,
                Status = WorkOrderStatus.New,
                OperatorName = "系统",
                Summary = "工单已创建并进入调度池",
                EventTime = workOrder.CreationTime
            }
        };

        if (workOrder.Status >= WorkOrderStatus.Assigned)
        {
            items.Add(new WorkOrderEvent
            {
                WorkOrderId = workOrder.Id,
                Status = WorkOrderStatus.Assigned,
                OperatorName = "调度中心",
                Summary = $"分派给 {workOrder.OwnerName}",
                EventTime = workOrder.CreationTime.AddMinutes(18)
            });
        }

        if (workOrder.Status >= WorkOrderStatus.InProgress)
        {
            items.Add(new WorkOrderEvent
            {
                WorkOrderId = workOrder.Id,
                Status = WorkOrderStatus.InProgress,
                OperatorName = workOrder.OwnerName,
                Summary = "现场处理中，已同步初步反馈",
                EventTime = workOrder.CreationTime.AddHours(2)
            });
        }

        if (workOrder.Status is WorkOrderStatus.Resolved or WorkOrderStatus.Closed)
        {
            items.Add(new WorkOrderEvent
            {
                WorkOrderId = workOrder.Id,
                Status = WorkOrderStatus.Resolved,
                OperatorName = workOrder.OwnerName,
                Summary = "问题已处理，等待复核",
                EventTime = workOrder.CompletedAt ?? workOrder.CreationTime.AddHours(4)
            });
        }

        return items;
    }

    private static List<WorkOrder> GetEnglishWorkOrders()
    {
        var sequence = 1;
        var now = DateTime.Now;

        return new Faker<WorkOrder>("en")
            .UseSeed(20260619)
            .RuleFor(x => x.Code, _ => $"EN-WO-{sequence++.ToString().PadLeft(5, '0')}")
            .RuleFor(x => x.Title, f => f.PickRandom(
                    "Preventive inspection",
                    "Temperature anomaly",
                    "Customer site support",
                    "Safety review",
                    "Asset maintenance",
                    "Network stability check"
                ) + " - " + f.Commerce.ProductName())
            .RuleFor(x => x.Type, f => f.PickRandom<WorkOrderType>())
            .RuleFor(x => x.Status, f => f.PickRandom<WorkOrderStatus>())
            .RuleFor(x => x.Priority, f => f.PickRandom<OperationPriority>())
            .RuleFor(x => x.OwnerName, f => f.PickRandom(EnglishOwners))
            .RuleFor(x => x.Location, f => f.PickRandom(EnglishLocations))
            .RuleFor(x => x.PlannedAt, f => now.Date.AddDays(f.Random.Int(-7, 10)).AddHours(f.Random.Int(7, 21)))
            .RuleFor(x => x.CompletedAt, (_, x) => x.Status is WorkOrderStatus.Resolved or WorkOrderStatus.Closed ? x.PlannedAt.AddHours(1 + sequence % 5) : null)
            .RuleFor(x => x.Description, f => f.PickRandom(
                "Customer-facing service window requires coordinated response and follow-up notes.",
                "Field team should verify device telemetry, local alarms, and maintenance checklist items.",
                "Operations desk opened the case after a dashboard signal exceeded the configured threshold.",
                "Shift owner requested a documented review before closing the incident."
            ))
            .RuleFor(x => x.CreationTime, f => now.AddDays(-f.Random.Int(0, 18)).AddMinutes(-f.Random.Int(0, 900)))
            .Generate(48);
    }

    private static List<WorkOrderEvent> GetEnglishEvents(WorkOrder workOrder)
    {
        var items = new List<WorkOrderEvent>
        {
            new()
            {
                WorkOrderId = workOrder.Id,
                Status = WorkOrderStatus.New,
                OperatorName = "Operations Desk",
                Summary = "Work order created and queued for dispatch.",
                EventTime = workOrder.CreationTime
            }
        };

        if (workOrder.Status >= WorkOrderStatus.Assigned)
        {
            items.Add(new WorkOrderEvent
            {
                WorkOrderId = workOrder.Id,
                Status = WorkOrderStatus.Assigned,
                OperatorName = "Dispatch Center",
                Summary = $"Assigned to {workOrder.OwnerName}.",
                EventTime = workOrder.CreationTime.AddMinutes(15)
            });
        }

        if (workOrder.Status >= WorkOrderStatus.InProgress)
        {
            items.Add(new WorkOrderEvent
            {
                WorkOrderId = workOrder.Id,
                Status = WorkOrderStatus.InProgress,
                OperatorName = workOrder.OwnerName,
                Summary = "Field response started and initial findings were synced.",
                EventTime = workOrder.CreationTime.AddHours(2)
            });
        }

        if (workOrder.Status is WorkOrderStatus.Resolved or WorkOrderStatus.Closed)
        {
            items.Add(new WorkOrderEvent
            {
                WorkOrderId = workOrder.Id,
                Status = WorkOrderStatus.Resolved,
                OperatorName = workOrder.OwnerName,
                Summary = "Issue resolved. Awaiting quality review and final confirmation.",
                EventTime = workOrder.CompletedAt ?? workOrder.CreationTime.AddHours(4)
            });
        }

        return items;
    }

    private static List<OperationAsset> GetAssets()
    {
        var sequence = 1;
        var now = DateTime.Now;

        return new Faker<OperationAsset>("zh_CN")
            .UseSeed(20260618)
            .RuleFor(x => x.Code, _ => $"AST-{sequence++.ToString().PadLeft(4, '0')}")
            .RuleFor(x => x.Name, f => f.PickRandom(AssetCategories) + "-" + f.Random.AlphaNumeric(4).ToUpperInvariant())
            .RuleFor(x => x.Category, f => f.PickRandom(AssetCategories))
            .RuleFor(x => x.Location, f => f.PickRandom(Locations))
            .RuleFor(x => x.Status, f => f.PickRandom<AssetStatus>())
            .RuleFor(x => x.HealthScore, f => f.Random.Int(45, 100))
            .RuleFor(x => x.LastInspectionTime, f => now.AddDays(-f.Random.Int(0, 30)))
            .Generate(72);
    }

    private static List<OperationAsset> GetEnglishAssets()
    {
        var sequence = 1;
        var now = DateTime.Now;

        return new Faker<OperationAsset>("en")
            .UseSeed(20260620)
            .RuleFor(x => x.Code, _ => $"EN-AST-{sequence++.ToString().PadLeft(4, '0')}")
            .RuleFor(x => x.Name, f => f.PickRandom(EnglishAssetCategories) + "-" + f.Random.AlphaNumeric(4).ToUpperInvariant())
            .RuleFor(x => x.Category, f => f.PickRandom(EnglishAssetCategories))
            .RuleFor(x => x.Location, f => f.PickRandom(EnglishLocations))
            .RuleFor(x => x.Status, f => f.PickRandom<AssetStatus>())
            .RuleFor(x => x.HealthScore, f => f.Random.Int(42, 100))
            .RuleFor(x => x.LastInspectionTime, f => now.AddDays(-f.Random.Int(0, 35)))
            .Generate(36);
    }

    private static List<OperationShift> GetShifts()
    {
        var shifts = new List<OperationShift>();
        var start = DateTime.Now.Date.AddDays(-10);

        for (var i = 0; i < 32; i++)
        {
            shifts.Add(new OperationShift
            {
                ShiftDate = start.AddDays(i),
                ShiftType = (OperationShiftType)(i % 3),
                OwnerName = Owners[i % Owners.Length],
                Responsibility = $"{Locations[i % Locations.Length]} 巡检与工单响应",
                WorkOrderCount = 2 + i % 7
            });
        }

        return shifts;
    }

    private static List<OperationShift> GetEnglishShifts()
    {
        var shifts = new List<OperationShift>();
        var start = DateTime.Now.Date.AddDays(-5);

        for (var i = 0; i < 16; i++)
        {
            shifts.Add(new OperationShift
            {
                ShiftDate = start.AddDays(i),
                ShiftType = (OperationShiftType)(i % 3),
                OwnerName = EnglishOwners[i % EnglishOwners.Length],
                Responsibility = $"{EnglishLocations[i % EnglishLocations.Length]} patrol, incident triage, and work order follow-up",
                WorkOrderCount = 1 + i % 6
            });
        }

        return shifts;
    }
}
