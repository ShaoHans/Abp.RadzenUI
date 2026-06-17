using CRM.Enums;
using Radzen;

namespace CRM.Blazor.Web.Components.Pages.Operations;

public static class OperationUi
{
    public static BadgeStyle GetStatusStyle(WorkOrderStatus status)
    {
        return status switch
        {
            WorkOrderStatus.New => BadgeStyle.Secondary,
            WorkOrderStatus.Assigned => BadgeStyle.Info,
            WorkOrderStatus.InProgress => BadgeStyle.Primary,
            WorkOrderStatus.Paused => BadgeStyle.Warning,
            WorkOrderStatus.Resolved => BadgeStyle.Success,
            WorkOrderStatus.Closed => BadgeStyle.Success,
            _ => BadgeStyle.Secondary
        };
    }

    public static BadgeStyle GetPriorityStyle(OperationPriority priority)
    {
        return priority switch
        {
            OperationPriority.Low => BadgeStyle.Secondary,
            OperationPriority.Normal => BadgeStyle.Info,
            OperationPriority.High => BadgeStyle.Warning,
            OperationPriority.Critical => BadgeStyle.Danger,
            _ => BadgeStyle.Secondary
        };
    }

    public static BadgeStyle GetAssetStyle(AssetStatus status)
    {
        return status switch
        {
            AssetStatus.Online => BadgeStyle.Success,
            AssetStatus.Warning => BadgeStyle.Warning,
            AssetStatus.Offline => BadgeStyle.Danger,
            AssetStatus.Maintenance => BadgeStyle.Info,
            _ => BadgeStyle.Secondary
        };
    }

    public static string GetHealthColor(int score)
    {
        return score switch
        {
            >= 90 => "var(--rz-success)",
            >= 75 => "var(--rz-info)",
            >= 60 => "var(--rz-warning)",
            _ => "var(--rz-danger)"
        };
    }
}
