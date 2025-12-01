using System.ComponentModel.DataAnnotations;
using Volo.Abp.Identity;
using Volo.Abp.ObjectExtending;
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
                }
            );

            ObjectExtensionManager.Instance.AddOrUpdateProperty<IdentityUserUpdateDto, string>(
                "HireDate",
                propertyInfo =>
                {
                    propertyInfo.Attributes.Add(new RequiredAttribute());
                }
            );
        });
    }
}
