using Abp.RadzenUI.Localization;
using Abp.RadzenUI.Permissions;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.UI.Navigation;
using static Abp.RadzenUI.Menus.RadzenUI;

namespace Abp.RadzenUI.Menus;

public class DataDictionaryMenuContributor : IMenuContributor
{
    public virtual Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name != StandardMenus.Main)
        {
            return Task.CompletedTask;
        }

        var administrationMenu = context.Menu.GetAdministration();

        var l = context.GetLocalizer<AbpRadzenUIResource>();

        var dataDictionaryMenuItem = new ApplicationMenuItem(
            DataDictionaryMenuNames.Default,
            l["Menu:DataDictionary"],
            url: "/data-dictionary",
            icon: "book"
        ).RequirePermissions(RadzenUIPermissions.DataDictionary.Default);
        administrationMenu.AddItem(dataDictionaryMenuItem);
        return Task.CompletedTask;
    }
}
