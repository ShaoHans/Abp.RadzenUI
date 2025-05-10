using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Radzen;
using Volo.Abp.MultiTenancy;

namespace Abp.RadzenUI.Components.Layout;

public partial class LoginLayout
{
    [Inject]
    ICurrentTenant? CurrentTenant { get; set; }

    [Inject]
    NavigationManager Navigation { get; set; } = default!;

    [Inject]
    IAuthenticationSchemeProvider SchemeProvider { get; set; } = default!;

    List<ExternalProviderModel> ExternalProviders { get; set; } = [];

    protected override async Task OnInitializedAsync()
    {
        ExternalProviders = await GetExternalProviders();
        //await base.OnInitializedAsync();
    }

    async Task OpenTenantSwitchDialog()
    {
        await DialogService.OpenAsync<Pages.Account.TenantSwitchDialog>(
            title: TL["SwitchTenant"],
            parameters: new Dictionary<string, object>
            {
                { "TenantName", CurrentTenant?.Name ?? string.Empty }
            },
            options: new DialogOptions
            {
                Draggable = true,
                Width = "550px",
                Height = "400px"
            }
        );
    }

    async Task<List<ExternalProviderModel>> GetExternalProviders()
    {
        var schemes = await SchemeProvider.GetAllSchemesAsync();

        return
        [
            .. schemes
                .Where(x => x.DisplayName != null)
                .Select(x => new ExternalProviderModel
                {
                    DisplayName = x.DisplayName,
                    AuthenticationScheme = x.Name
                })
        ];
    }
}

public class ExternalProviderModel
{
    public string? DisplayName { get; set; }
    public string AuthenticationScheme { get; set; } = default!;
}
