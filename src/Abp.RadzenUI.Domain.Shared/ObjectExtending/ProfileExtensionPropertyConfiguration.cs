namespace Abp.RadzenUI.ObjectExtending;

/// <summary>
/// Per-property settings for the profile page. <c>null</c> members fall back to
/// <see cref="AbpRadzenUIProfileOptions"/>.
/// </summary>
public class ProfileExtensionPropertyConfiguration
{
    public ProfileExtensionPropertyMode? Mode { get; set; }
}
