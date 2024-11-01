using Volo.Abp.Reflection;

namespace Abp.RadzenUI.Permissions;

public static class RadzenUIPermissions
{
    public const string GroupName = "AbpRadzenUI";

    public static class AuditLogs
    {
        public const string Default = GroupName + ".AuditLogs";
    }
    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(RadzenUIPermissions));
    }
}
