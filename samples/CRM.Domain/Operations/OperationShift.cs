using System;
using CRM.Enums;
using Volo.Abp.Domain.Entities.Auditing;

namespace CRM.Operations;

public class OperationShift : AuditedAggregateRoot<Guid>
{
    public DateTime ShiftDate { get; set; }

    public OperationShiftType ShiftType { get; set; }

    public string OwnerName { get; set; } = default!;

    public string Responsibility { get; set; } = default!;

    public int WorkOrderCount { get; set; }
}
