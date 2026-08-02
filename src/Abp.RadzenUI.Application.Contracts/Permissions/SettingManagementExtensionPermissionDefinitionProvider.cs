using Abp.RadzenUI.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.SettingManagement;

namespace Abp.RadzenUI.Permissions;

public class SettingManagementExtensionPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var moduleGroup = context.GetGroup(SettingManagementPermissions.GroupName);

        moduleGroup.AddPermission(
            SettingManagementExtensionPermissions.Account,
            L("Permission:SettingManagement_Account")
        );
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<AbpRadzenUIResource>(name);
    }
}
