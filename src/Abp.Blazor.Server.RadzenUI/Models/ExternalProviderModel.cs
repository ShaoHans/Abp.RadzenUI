namespace Abp.RadzenUI.Models;

internal class ExternalProviderModel
{
    public string? DisplayName { get; set; }
    public string AuthenticationScheme { get; set; } = default!;
    public string IconPath { get; set; } = string.Empty;
}
