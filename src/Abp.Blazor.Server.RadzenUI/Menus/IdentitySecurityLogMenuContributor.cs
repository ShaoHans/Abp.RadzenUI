using Abp.RadzenUI.Localization;
using Abp.RadzenUI.Permissions;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.UI.Navigation;
using static Abp.RadzenUI.Menus.RadzenUI;

namespace Abp.RadzenUI.Menus;

public class IdentitySecurityLogMenuContributor : IMenuContributor
{
    public virtual Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name != StandardMenus.Main)
        {
            return Task.CompletedTask;
        }

        var administrationMenu = context.Menu.GetAdministration();
        var l = context.GetLocalizer<AbpRadzenUIResource>();

        var securityLogMenuItem = new ApplicationMenuItem(
            SecurityLogMenuNames.Default,
            l["Menu:SecurityLogs"],
            url: "/identity/security-logs",
            icon: "login"
        ).RequirePermissions(RadzenUIPermissions.SecurityLogs.Default);

        administrationMenu.AddItem(securityLogMenuItem);

        return Task.CompletedTask;
    }
}