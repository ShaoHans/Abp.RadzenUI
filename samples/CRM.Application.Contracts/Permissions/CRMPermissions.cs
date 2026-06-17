using Volo.Abp.Reflection;

namespace CRM.Permissions;

public static class CRMPermissions
{
    public const string GroupName = "CRM";

    public static class Products
    {
        public const string Default = GroupName + ".Products";
        public const string Delete = Default + ".Delete";
        public const string Update = Default + ".Update";
        public const string Create = Default + ".Create";
    }

    public static class Operations
    {
        public const string Default = GroupName + ".Operations";
        public const string Dashboard = Default + ".Dashboard";
        public const string WorkOrders = Default + ".WorkOrders";
        public const string WorkOrderActions = WorkOrders + ".Actions";
        public const string Assets = Default + ".Assets";
        public const string Shifts = Default + ".Shifts";
    }

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(CRMPermissions));
    }
}
