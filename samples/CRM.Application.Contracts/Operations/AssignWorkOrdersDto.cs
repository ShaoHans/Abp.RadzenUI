using System;
using System.Collections.Generic;

namespace CRM.Operations;

public class AssignWorkOrdersDto
{
    public List<Guid> WorkOrderIds { get; set; } = [];

    public string OwnerName { get; set; } = default!;
}
