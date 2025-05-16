using System.ComponentModel.DataAnnotations;
using Volo.Abp;

namespace Abp.RadzenUI.Blazor.SettingManagement;

public class SettingComponentGroup
{
    public const int DefaultOrder = 1000;

    public string Id { get; set; } = default!;

    public string DisplayName { get; set; } = default!;

    public Type ComponentType { get; set; } = default!;

    public object? Parameter { get; set; }

    public int Order { get; set; }

    public SettingComponentGroup(
        [Required] string id,
        [Required] string displayName,
        [Required] Type componentType,
        object? parameter = null,
        int order = DefaultOrder
    )
    {
        Id = id;
        DisplayName = displayName;
        ComponentType = componentType;
        Parameter = parameter;
        Order = order;
    }
}
