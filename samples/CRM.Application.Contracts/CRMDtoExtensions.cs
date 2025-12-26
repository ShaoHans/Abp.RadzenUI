using System.ComponentModel.DataAnnotations;
using Volo.Abp.Identity;
using Volo.Abp.ObjectExtending;
using Volo.Abp.TenantManagement;
using Volo.Abp.Threading;

namespace CRM;

public static class CRMDtoExtensions
{
    private static readonly OneTimeRunner OneTimeRunner = new();

    public static void Configure()
    {
        OneTimeRunner.Run(() =>
        {
            ObjectExtensionManager.Instance.AddOrUpdateProperty<IdentityUserCreateDto, string>(
                "HireDate",
                propertyInfo =>
                {
                    propertyInfo.Attributes.Add(new RequiredAttribute());
                    propertyInfo.Attributes.Add(new DataTypeAttribute(DataType.Date));
                    propertyInfo.CheckPairDefinitionOnMapping = false;
                }
            );

            ObjectExtensionManager.Instance.AddOrUpdateProperty<IdentityUserUpdateDto, string>(
                "HireDate",
                propertyInfo =>
                {
                    propertyInfo.Attributes.Add(new RequiredAttribute());
                    propertyInfo.Attributes.Add(new DataTypeAttribute(DataType.Date));
                    propertyInfo.CheckPairDefinitionOnMapping = false;
                }
            );

            ObjectExtensionManager.Instance.AddOrUpdateProperty<IdentityUserDto, string>(
                "HireDate",
                propertyInfo =>
                {
                    propertyInfo.Configuration.Add("Width", "150px");
                    propertyInfo.Configuration.Add("LocalizationKey", "DisplayName:HireDate");
                    //propertyInfo.Configuration.Add("Title", "DisplayName");
                    propertyInfo.Configuration.Add("FormatString", "{0:yyyy-MM-dd}");
                    propertyInfo.CheckPairDefinitionOnMapping = false;
                }
            );

            ObjectExtensionManager.Instance.AddOrUpdateProperty<TenantCreateDto, string>(
                "TenantDisplayName",
                propertyInfo =>
                {
                    propertyInfo.Attributes.Add(new DataTypeAttribute(DataType.Text));
                    propertyInfo.CheckPairDefinitionOnMapping = false;
                }
            );

            ObjectExtensionManager.Instance.AddOrUpdateProperty<TenantUpdateDto, string>(
                "TenantDisplayName",
                propertyInfo =>
                {
                    propertyInfo.Attributes.Add(new DataTypeAttribute(DataType.Text));
                    propertyInfo.CheckPairDefinitionOnMapping = false;
                }
            );

            ObjectExtensionManager.Instance.AddOrUpdateProperty<TenantDto, string>(
                "TenantDisplayName",
                propertyInfo =>
                {
                    propertyInfo.Configuration.Add("Width", "200px");
                    propertyInfo.Configuration.Add("LocalizationKey", "DisplayName:TenantDisplayName");
                    //propertyInfo.Configuration.Add("Title", "DisplayName");
                    propertyInfo.Configuration.Add("FormatString", "");
                    propertyInfo.CheckPairDefinitionOnMapping = false;
                }
            );
        });
    }
}
