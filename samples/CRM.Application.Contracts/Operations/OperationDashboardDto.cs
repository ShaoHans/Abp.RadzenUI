using System.Collections.Generic;

namespace CRM.Operations;

public class OperationDashboardDto
{
    public int TotalWorkOrders { get; set; }

    public int OpenWorkOrders { get; set; }

    public int CriticalWorkOrders { get; set; }

    public int OnlineAssets { get; set; }

    public decimal AverageHealthScore { get; set; }

    public List<OperationMetricDto> WorkOrdersByStatus { get; set; } = [];

    public List<OperationMetricDto> WorkOrdersByPriority { get; set; } = [];

    public List<OperationMetricDto> AssetHealthBands { get; set; } = [];

    public List<OperationMetricDto> LastSevenDays { get; set; } = [];
}
