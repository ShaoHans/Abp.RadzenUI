using CRM.Localization;
using CRM.MultiTenancy;
using CRM.Permissions;

using Microsoft.Extensions.Localization;

using Volo.Abp.Authorization.Permissions;
using Volo.Abp.UI.Navigation;

using static Abp.RadzenUI.Menus.RadzenUI;

namespace CRM.Blazor.Web.Menus;

public class CRMMenuContributor : IMenuContributor
{
    public async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name == StandardMenus.Main)
        {
            await ConfigureMainMenuAsync(context);
        }
    }

    private Task ConfigureMainMenuAsync(MenuConfigurationContext context)
    {
        var l = context.GetLocalizer<CRMResource>();

        context.Menu.Items.Insert(
            0,
            new ApplicationMenuItem(
                CRMMenus.Home,
                l["Menu:Home"],
                "/",
                icon: "home",
                order: 1
            )
        );

        ConfigProductMenu(context, l);

        //Administration
        var administration = context.Menu.GetAdministration();
        administration.Order = 100;

        if (MultiTenancyConsts.IsEnabled)
        {
            administration.SetSubItemOrder(TenantManagementMenuNames.GroupName, 1);
        }
        else
        {
            administration.TryRemoveMenuItem(TenantManagementMenuNames.GroupName);
        }

        administration.SetSubItemOrder(IdentityMenuNames.GroupName, 2);
        //administration.SetSubItemOrder(SettingManagementMenus.GroupName, 3);

        return Task.CompletedTask;
    }

    private void ConfigProductMenu(MenuConfigurationContext context, IStringLocalizer l)
    {
        var productMenu = new ApplicationMenuItem(
                CRMMenus.Product,
                l["Menu:Product"],
                icon: "inventory_2",
                order: 2
            );

        productMenu.AddItem(
            new ApplicationMenuItem(
                CRMMenus.ProductList,
                l["Menu:Product.List"],
                "/products",
                order: 1
            ).RequirePermissions(CRMPermissions.Products.Default)
        );

        context.Menu.Items.Add(productMenu);
    }
}
