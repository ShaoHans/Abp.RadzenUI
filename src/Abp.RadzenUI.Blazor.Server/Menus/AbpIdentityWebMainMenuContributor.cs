using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Identity;
using Volo.Abp.Identity.Localization;
using Volo.Abp.UI.Navigation;

namespace Abp.RadzenUI.Menus;

public class AbpIdentityWebMainMenuContributor : IMenuContributor
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
            icon: "security_key"
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

        return Task.CompletedTask;
    }
}
