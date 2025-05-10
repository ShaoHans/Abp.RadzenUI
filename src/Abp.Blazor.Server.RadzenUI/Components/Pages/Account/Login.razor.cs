using Microsoft.AspNetCore.Components;
using Radzen;
using Volo.Abp.Account.Localization;

namespace Abp.RadzenUI.Components.Pages.Account;

public partial class Login
{
    [SupplyParameterFromQuery]
    [Parameter]
    public string? Error { get; set; }

    [Inject]
    NavigationManager Navigation { get; set; }

    public Login()
    {
        LocalizationResource = typeof(AccountResource);
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if (!string.IsNullOrWhiteSpace(Error))
        {
            await Message.Error(Error);
        }

        if (CurrentUser != null && CurrentUser.IsAuthenticated)
        {
            NavigationManager.NavigateTo("/");
        }
    }

    //async Task OpenTenantSwitchDialog()
    //{
    //    await DialogService.OpenAsync<TenantSwitchDialog>(
    //        title: TL["SwitchTenant"],
    //        parameters: new Dictionary<string, object>
    //        {
    //            { "TenantName", CurrentTenant?.Name ?? string.Empty }
    //        },
    //        options: new DialogOptions
    //        {
    //            Draggable = true,
    //            Width = "550px",
    //            Height = "400px"
    //        }
    //    );
    //}

    //private void LoginAzure()
    //{
    //    // ��ת�� ABP ���ⲿ��¼��ַ��provider=AzureOpenId ��Ӧ�������õķ�����
    //    var returnUrl = Navigation.Uri; // ��¼�󷵻ص�ǰҳ��
    //    var encoded = Uri.EscapeDataString(returnUrl);
    //    Navigation.NavigateTo(
    //        $"Account/ExternalLogin?provider=AzureOpenId&returnUrl={encoded}",
    //        forceLoad: true
    //    );
    //}
}
