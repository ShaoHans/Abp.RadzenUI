using System;
using CRM.Enums;
using Volo.Abp.Application.Dtos;

namespace CRM.Operations;

public class WorkOrderEventDto : EntityDto<Guid>
{
    public Guid WorkOrderId { get; set; }

    public WorkOrderStatus Status { get; set; }

    public string OperatorName { get; set; } = default!;

    public string Summary { get; set; } = default!;

    public DateTime EventTime { get; set; }
}
