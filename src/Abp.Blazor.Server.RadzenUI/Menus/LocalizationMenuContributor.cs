using Abp.RadzenUI.Localization;
using Abp.RadzenUI.Permissions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.UI.Navigation;
using static Abp.RadzenUI.Menus.RadzenUI;

namespace Abp.RadzenUI.Menus;

public class LocalizationMenuContributor : IMenuContributor
{
    public virtual Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name != StandardMenus.Main)
        {
            return Task.CompletedTask;
        }

        var options = context.ServiceProvider.GetRequiredService<IOptions<AbpRadzenUIOptions>>().Value;
        if (!options.EnableLocalizationManagement)
        {
            return Task.CompletedTask;
        }

        var administrationMenu = context.Menu.GetAdministration();

        var l = context.GetLocalizer<AbpRadzenUIResource>();

        var localizationMenuItem = new ApplicationMenuItem(
            LocalizationMenuNames.Default,
            l["Menu:Localization"],
            url: "/localization-texts",
            icon: "translate"
        ).RequirePermissions(RadzenUIPermissions.Localization.Default);
        administrationMenu.AddItem(localizationMenuItem);
        return Task.CompletedTask;
    }
}
