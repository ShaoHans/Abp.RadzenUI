using CRM.Enums;

namespace CRM.Operations;

public class ChangeWorkOrderStatusDto
{
    public WorkOrderStatus Status { get; set; }

    public string? Summary { get; set; }
}
