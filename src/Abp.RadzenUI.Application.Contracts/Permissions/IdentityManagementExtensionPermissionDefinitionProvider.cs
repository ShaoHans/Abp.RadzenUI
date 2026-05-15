using Abp.RadzenUI.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Identity;
using Volo.Abp.Localization;
using static Abp.RadzenUI.Permissions.IdentityManagementExtensionPermissions;

namespace Abp.RadzenUI.Permissions;

public class IdentityManagementExtensionPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var identityGroup = context.GetGroup(IdentityPermissions.GroupName);
        var ouGroup = identityGroup.AddPermission(
            OrganizationUnits.Default,
            L("Permission:Ou.Default")
        );
        ouGroup.AddChild(OrganizationUnits.ManageTree, L("Permission:Ou.ManageTree"));
        ouGroup.AddChild(OrganizationUnits.ManageUsers, L("Permission:Ou.ManageUsers"));
        ouGroup.AddChild(OrganizationUnits.ManageRoles, L("Permission:Ou.ManageRoles"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<AbpRadzenUIResource>(name);
    }
}
