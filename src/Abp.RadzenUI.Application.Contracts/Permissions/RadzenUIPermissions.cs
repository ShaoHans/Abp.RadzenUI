using Volo.Abp.Reflection;

namespace Abp.RadzenUI.Permissions;

public static class RadzenUIPermissions
{
    public const string GroupName = "AbpRadzenUI";

    public static class AuditLogs
    {
        public const string Default = GroupName + ".AuditLogs";
    }

    public static class SecurityLogs
    {
        public const string Default = GroupName + ".SecurityLogs";
    }

    public static class DataDictionary
    {
        public const string Default = GroupName + ".DataDictionary";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
    }

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(RadzenUIPermissions));
    }
}
