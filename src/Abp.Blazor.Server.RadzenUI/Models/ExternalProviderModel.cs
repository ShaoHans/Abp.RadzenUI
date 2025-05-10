namespace Abp.RadzenUI.Models;

public class ExternalProviderModel
{
    public string? DisplayName { get; set; }
    public string AuthenticationScheme { get; set; } = default!;
}
