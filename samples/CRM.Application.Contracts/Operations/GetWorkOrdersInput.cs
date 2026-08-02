using System;
using CRM.Enums;
using Volo.Abp.Application.Dtos;

namespace CRM.Operations;

public class GetWorkOrdersInput : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }

    public WorkOrderStatus? Status { get; set; }

    public WorkOrderType? Type { get; set; }

    public OperationPriority? Priority { get; set; }

    public string? OwnerName { get; set; }

    public DateTime? PlannedStart { get; set; }

    public DateTime? PlannedEnd { get; set; }
}
