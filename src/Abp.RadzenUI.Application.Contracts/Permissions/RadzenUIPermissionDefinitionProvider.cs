using Abp.RadzenUI.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Abp.RadzenUI.Permissions;

public class RadzenUIPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var radzenUIGroup = context.AddGroup(RadzenUIPermissions.GroupName, L("Permission:SystemGroupName"));
        radzenUIGroup.AddPermission(RadzenUIPermissions.AuditLogs.Default, L("Permission:AuditLogs"));
        radzenUIGroup.AddPermission(RadzenUIPermissions.SecurityLogs.Default, L("Permission:SecurityLogs"));

        var dataDictionaryPermission = radzenUIGroup.AddPermission(RadzenUIPermissions.DataDictionary.Default, L("Permission:DataDictionary"));
        dataDictionaryPermission.AddChild(RadzenUIPermissions.DataDictionary.Create, L("Permission:DataDictionary.Create"));
        dataDictionaryPermission.AddChild(RadzenUIPermissions.DataDictionary.Update, L("Permission:DataDictionary.Update"));
        dataDictionaryPermission.AddChild(RadzenUIPermissions.DataDictionary.Delete, L("Permission:DataDictionary.Delete"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<AbpRadzenUIResource>(name);
    }
}
