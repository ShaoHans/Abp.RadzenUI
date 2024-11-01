using Abp.RadzenUI.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Abp.RadzenUI.Permissions;

public class RadzenUIPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var radzenUIGroup = context.AddGroup(RadzenUIPermissions.GroupName, L("Permission:GroupName"));
        var auditLogs = radzenUIGroup.AddPermission(RadzenUIPermissions.AuditLogs.Default, L("Permission:AuditLogs"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<AbpRadzenUIResource>(name);
    }
}
