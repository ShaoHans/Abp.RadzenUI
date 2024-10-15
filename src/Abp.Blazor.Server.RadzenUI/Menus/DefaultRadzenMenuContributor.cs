using Volo.Abp.UI.Navigation;
using Volo.Abp.UI.Navigation.Localization.Resource;

namespace Abp.RadzenUI.Menus;

public class DefaultRadzenMenuContributor : IMenuContributor
{
    public Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name != StandardMenus.Main)
        {
            return Task.CompletedTask;
        }

        var l = context.GetLocalizer<AbpUiNavigationResource>();

        context.Menu.AddItem(
                new ApplicationMenuItem(
                    DefaultMenuNames.Application.Main.Administration,
                    l["Menu:Administration"],
                    icon: "manage_accounts"
                )
            );

        return Task.CompletedTask;
    }
}
