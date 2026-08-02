using Abp.RadzenUI.Localization;
using Volo.Abp.UI.Navigation;
using static Abp.RadzenUI.Menus.RadzenUI;

namespace Abp.RadzenUI.Menus;

public class MessageMenuContributor : IMenuContributor
{
    public virtual Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name != StandardMenus.Main)
        {
            return Task.CompletedTask;
        }

        var administrationMenu = context.Menu.GetAdministration();
        var l = context.GetLocalizer<AbpRadzenUIResource>();

        administrationMenu.AddItem(
            new ApplicationMenuItem(
                MessageMenuNames.Default,
                l["Menu:Messages"],
                url: "/messages",
                icon: "notifications"
            )
        );

        return Task.CompletedTask;
    }
}