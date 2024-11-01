using Volo.Abp.AuditLogging.Localization;
using Volo.Abp.UI.Navigation;
using static Abp.RadzenUI.Menus.RadzenUI;

namespace Abp.RadzenUI.Menus;

public class AuditLoggingMenuContributor : IMenuContributor
{
    public virtual Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name != StandardMenus.Main)
        {
            return Task.CompletedTask;
        }

        var administrationMenu = context.Menu.GetAdministration();

        var l = context.GetLocalizer<AuditLoggingResource>();

        var auditLoggingMenuItem = new ApplicationMenuItem(
            AuditLoggingMenuNames.Default,
            l["Menu:AuditLogging"],
            url: "/auditlogs",
            icon: "description"
        );
        administrationMenu.AddItem(auditLoggingMenuItem);
        return Task.CompletedTask;
    }
}
