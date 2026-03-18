namespace Abp.RadzenUI.Menus;

public class RadzenUI
{
    public static class IdentityMenuNames
    {
        public const string GroupName = "AbpIdentity";

        public const string Roles = GroupName + ".Roles";
        public const string Users = GroupName + ".Users";
        public const string OrganizationUnits = GroupName + ".OrganizationUnits";
    }

    public static class TenantManagementMenuNames
    {
        public const string GroupName = "TenantManagement";

        public const string Tenants = GroupName + ".Tenants";
    }

    public static class AuditLoggingMenuNames
    {
        public const string Default = "AuditLogging";
    }

    public static class SecurityLogMenuNames
    {
        public const string Default = "SecurityLog";
    }

    public static class SettingManagementMenus
    {
        public const string GroupName = "SettingManagement";
    }

    public static class DataDictionaryMenuNames
    {
        public const string Default = "DataDictionary";
    }
}
