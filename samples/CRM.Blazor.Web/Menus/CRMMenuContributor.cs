using Abp.RadzenUI.Localization;
using Abp.RadzenUI.Infrastructure.Navigation;
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

    private static Task ConfigureMainMenuAsync(MenuConfigurationContext context)
    {
        var l = context.GetLocalizer<CRMResource>();

        context.Menu.Items.Insert(
            0,
            new ApplicationMenuItem(CRMMenus.Home, l["Menu:Home"], "/", icon: "home", order: 1)
                .WithIconColor("#2563eb")
        );

        var rl = context.GetLocalizer<AbpRadzenUIResource>();
        context.Menu.Items.Insert(
            1,
            new ApplicationMenuItem(CRMMenus.Dashboard, rl["Menu:Dashboard"], "/dashboard", icon: "dashboard", order: 2)
                .WithIconColor("#0f766e")
        );

        ConfigOperationMenu(context, l);
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
        administration.SetSubItemOrder(AuditLoggingMenuNames.Default, 3);

        return Task.CompletedTask;
    }

    private static void ConfigProductMenu(MenuConfigurationContext context, IStringLocalizer l)
    {
        var productMenu = new ApplicationMenuItem(
            CRMMenus.Product,
            l["Menu:Product"],
            icon: "inventory_2",
            order: 2
        ).WithIconColor("#ea580c");

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

    private static void ConfigOperationMenu(MenuConfigurationContext context, IStringLocalizer l)
    {
        var operationsMenu = new ApplicationMenuItem(
            CRMMenus.Operations,
            l["Menu:Operations"],
            icon: "support_agent",
            order: 3
        ).WithIconColor("#0f766e");

        operationsMenu.AddItem(
            new ApplicationMenuItem(
                CRMMenus.OperationsDashboard,
                l["Menu:Operations.Dashboard"],
                "/operations/dashboard",
                icon: "monitoring",
                order: 1
            ).RequirePermissions(CRMPermissions.Operations.Dashboard)
        );

        operationsMenu.AddItem(
            new ApplicationMenuItem(
                CRMMenus.WorkOrders,
                l["Menu:Operations.WorkOrders"],
                "/operations/work-orders",
                icon: "fact_check",
                order: 2
            ).RequirePermissions(CRMPermissions.Operations.WorkOrders)
        );

        operationsMenu.AddItem(
            new ApplicationMenuItem(
                CRMMenus.WorkOrderBoard,
                l["Menu:Operations.Board"],
                "/operations/board",
                icon: "view_kanban",
                order: 3
            ).RequirePermissions(CRMPermissions.Operations.WorkOrders)
        );

        operationsMenu.AddItem(
            new ApplicationMenuItem(
                CRMMenus.OperationShifts,
                l["Menu:Operations.Shifts"],
                "/operations/shifts",
                icon: "calendar_month",
                order: 4
            ).RequirePermissions(CRMPermissions.Operations.Shifts)
        );

        operationsMenu.AddItem(
            new ApplicationMenuItem(
                CRMMenus.OperationAssets,
                l["Menu:Operations.Assets"],
                "/operations/assets",
                icon: "precision_manufacturing",
                order: 5
            ).RequirePermissions(CRMPermissions.Operations.Assets)
        );

        context.Menu.Items.Add(operationsMenu);
    }
}
