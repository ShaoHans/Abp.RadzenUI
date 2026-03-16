using System.ComponentModel.DataAnnotations;
using Volo.Abp.Account;
using Volo.Abp.Identity;
using Volo.Abp.ObjectExtending;
using Volo.Abp.Threading;

namespace Abp.RadzenUI.Avatar;

public static class AvatarModuleExtensionConfigurator
{
    private static readonly OneTimeRunner OneTimeRunner = new();

    public static void Configure()
    {
        OneTimeRunner.Run(() =>
        {
            ObjectExtensionManager.Instance.Modules().ConfigureIdentity(identity =>
            {
                identity.ConfigureUser(user =>
                {
                    user.AddOrUpdateProperty<string>(
                        AvatarConsts.ExtraPropertyName,
                        property =>
                        {
                            property.Attributes.Add(new DataTypeAttribute(DataType.ImageUrl));
                        }
                    );
                });
            });

            ObjectExtensionManager.Instance.AddOrUpdateProperty<ProfileDto, string>(
                AvatarConsts.ExtraPropertyName,
                property => property.CheckPairDefinitionOnMapping = false
            );
        });
    }
}