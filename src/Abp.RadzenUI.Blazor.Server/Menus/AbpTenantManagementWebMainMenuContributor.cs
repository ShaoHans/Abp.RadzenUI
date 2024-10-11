using Volo.Abp.Authorization.Permissions;
using Volo.Abp.TenantManagement.Localization;
using Volo.Abp.UI.Navigation;
using Volo.Abp.TenantManagement;

namespace Abp.RadzenUI.Menus;

public class AbpTenantManagementWebMainMenuContributor : IMenuContributor
{
    public virtual Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name != StandardMenus.Main)
        {
            return Task.CompletedTask;
        }

        var administrationMenu = context.Menu.GetAdministration();

        var l = context.GetLocalizer<AbpTenantManagementResource>();

        var tenantManagementMenuItem = new ApplicationMenuItem(
            RadzenUI.TenantManagementMenuNames.GroupName,
            l["Menu:TenantManagement"],
            icon: "tenant"
        );
        administrationMenu.AddItem(tenantManagementMenuItem);

        tenantManagementMenuItem.AddItem(
            new ApplicationMenuItem(
                RadzenUI.TenantManagementMenuNames.Tenants,
                l["Tenants"],
                url: "/tenantManagement/tenants"
            ).RequirePermissions(TenantManagementPermissions.Tenants.Default)
        );

        return Task.CompletedTask;
    }
}
