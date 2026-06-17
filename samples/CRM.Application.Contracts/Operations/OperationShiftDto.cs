using System;
using CRM.Enums;
using Volo.Abp.Application.Dtos;

namespace CRM.Operations;

public class OperationShiftDto : EntityDto<Guid>
{
    public DateTime ShiftDate { get; set; }

    public OperationShiftType ShiftType { get; set; }

    public string OwnerName { get; set; } = default!;

    public string Responsibility { get; set; } = default!;

    public int WorkOrderCount { get; set; }
}
