using System.Collections.Generic;

namespace CRM.Operations;

public class WorkOrderDetailDto
{
    public WorkOrderDto WorkOrder { get; set; } = default!;

    public List<WorkOrderEventDto> Events { get; set; } = [];
}
