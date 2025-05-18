using System.ComponentModel.DataAnnotations;

namespace Abp.RadzenUI.Blazor.SettingManagement;

public class SettingComponentGroup(
    [Required] string id,
    [Required] string displayName,
    [Required] Type componentType,
    object? parameter = null,
    int order = SettingComponentGroup.DefaultOrder
)
{
    public const int DefaultOrder = 1000;

    public string Id { get; set; } = id;

    public string Key { get; set; } = id.Replace('.', '-').Replace('/', '-').ToLower();

    public string DisplayName { get; set; } = displayName;

    public Type ComponentType { get; set; } = componentType;

    public object? Parameter { get; set; } = parameter;

    public int Order { get; set; } = order;
}
