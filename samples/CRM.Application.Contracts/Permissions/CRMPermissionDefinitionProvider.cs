using CRM.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace CRM.Permissions;

public class CRMPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        //var myGroup = context.AddGroup(CRMPermissions.GroupName);
        
        var productManagementGroup = context.AddGroup(CRMPermissions.GroupName, L("Permission:GroupName"));
        var products = productManagementGroup.AddPermission(CRMPermissions.Products.Default, L("Permission:Products"));
        products.AddChild(CRMPermissions.Products.Update, L("Permission:Products.Edit"));
        products.AddChild(CRMPermissions.Products.Delete, L("Permission:Products.Delete"));
        products.AddChild(CRMPermissions.Products.Create, L("Permission:Products.Create"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<CRMResource>(name);
    }
}
