using Volo.Abp.Identity;
using Volo.Abp.Reflection;

namespace Abp.RadzenUI.Permissions;

public class IdentityManagementExtensionPermissions
{
    public static class OrganizationUnits
    {
        public const string Default = IdentityPermissions.GroupName + ".OrganizationUnits";
        public const string ManageTree = Default + ".ManageTree";
        public const string ManageUsers = Default + ".ManageUsers";
        public const string ManageRoles = Default + ".ManageRoles";
    }

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(
            typeof(IdentityManagementExtensionPermissions)
        );
    }
}
