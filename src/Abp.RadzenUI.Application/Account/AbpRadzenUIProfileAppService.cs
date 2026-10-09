using Abp.RadzenUI.ObjectExtending;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Volo.Abp.Account;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Identity;
using Volo.Abp.ObjectExtending;

namespace Abp.RadzenUI.Account;

/// <summary>
/// Replaces ABP's <see cref="ProfileAppService"/> so that extension properties whose
/// <see cref="ProfileExtensionPropertyMode"/> is not <see cref="ProfileExtensionPropertyMode.Editable"/>
/// cannot be changed through the profile endpoint, even by calling the API directly.
/// </summary>
[Dependency(ReplaceServices = true)]
[ExposeServices(typeof(IProfileAppService), typeof(ProfileAppService))]
public class AbpRadzenUIProfileAppService(
    IdentityUserManager userManager,
    IOptions<IdentityOptions> identityOptions,
    IOptions<AbpRadzenUIProfileOptions> profileOptions
) : ProfileAppService(userManager, identityOptions)
{
    protected AbpRadzenUIProfileOptions ProfileOptions { get; } = profileOptions.Value;

    public override async Task<ProfileDto> UpdateAsync(UpdateProfileDto input)
    {
        var properties = await ObjectExtensionManager.Instance.GetPropertiesAndCheckPolicyAsync<UpdateProfileDto>(
            LazyServiceProvider
        );

        ProfileExtensionPropertyHelper.RemoveNonEditableProperties(input, properties, ProfileOptions);

        return await base.UpdateAsync(input);
    }
}
