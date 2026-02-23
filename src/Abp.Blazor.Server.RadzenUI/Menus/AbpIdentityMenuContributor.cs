using Abp.RadzenUI.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Identity;
using Volo.Abp.Identity.Localization;
using Volo.Abp.UI.Navigation;

namespace Abp.RadzenUI.Menus;

public class AbpIdentityMenuContributor : IMenuContributor
{
    public virtual Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name != StandardMenus.Main)
        {
            return Task.CompletedTask;
        }

        var administrationMenu = context.Menu.GetAdministration();

        var l = context.GetLocalizer<IdentityResource>();

        var identityMenuItem = new ApplicationMenuItem(
            RadzenUI.IdentityMenuNames.GroupName,
            l["Menu:IdentityManagement"],
            icon: "id_card"
        );
        administrationMenu.AddItem(identityMenuItem);

        identityMenuItem.AddItem(
            new ApplicationMenuItem(
                RadzenUI.IdentityMenuNames.Roles,
                l["Roles"],
                url: "/identity/roles"
            ).RequirePermissions(IdentityPermissions.Roles.Default)
        );

        identityMenuItem.AddItem(
            new ApplicationMenuItem(
                RadzenUI.IdentityMenuNames.Users,
                l["Users"],
                url: "/identity/users"
            ).RequirePermissions(IdentityPermissions.Users.Default)
        );

        var rl = context.GetLocalizer<AbpRadzenUIResource>();

        identityMenuItem.AddItem(
            new ApplicationMenuItem(
                RadzenUI.IdentityMenuNames.OrganizationUnits,
                rl["OrganizationUnits"],
                url: "/identity/organization-units"
            ).RequirePermissions(
                Permissions.IdentityManagementExtensionPermissions.OrganizationUnits.Default
            )
        );

        return Task.CompletedTask;
    }
}
