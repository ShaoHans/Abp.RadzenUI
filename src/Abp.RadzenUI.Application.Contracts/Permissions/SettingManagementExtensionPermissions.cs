using Volo.Abp.Reflection;
using Volo.Abp.SettingManagement;

namespace Abp.RadzenUI.Permissions;

public class SettingManagementExtensionPermissions
{
    public const string Account = SettingManagementPermissions.GroupName + ".Account";

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(
            typeof(SettingManagementExtensionPermissions)
        );
    }
}
