using CRM.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Identity;
using Volo.Abp.UI.Navigation;

namespace Abp.RadzenUI.Menus;

public class DefaultRadzenMenuContributor : IMenuContributor
{
    public Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name != StandardMenus.Main)
        {
            return Task.CompletedTask;
        }

        var l = context.GetLocalizer<AbpRadzenUIResource>();

        var identityMenuItem = new ApplicationMenuItem(
            "Identity",
            "身份认证管理",
            icon: "security_key"
        );
        context.Menu.AddItem(identityMenuItem);

        identityMenuItem.AddItem(
            new ApplicationMenuItem(
                "Identity.Roles",
                "角色",
                url: "/role/list",
                icon: "safety_check"
            ).RequirePermissions(IdentityPermissions.Roles.Default)
        );

        identityMenuItem.AddItem(
            new ApplicationMenuItem(
                "Identity.Users",
                "用户",
                url: "/user/list",
                icon: "person"
            ).RequirePermissions(IdentityPermissions.Users.Default)
        );

        return Task.CompletedTask;
    }
}
