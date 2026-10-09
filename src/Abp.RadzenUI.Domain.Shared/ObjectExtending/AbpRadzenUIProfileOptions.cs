namespace Abp.RadzenUI.ObjectExtending;

/// <summary>
/// Options for extension properties shown on the user's profile page.
/// Configure via <c>Configure&lt;AbpRadzenUIProfileOptions&gt;(...)</c>.
/// </summary>
public class AbpRadzenUIProfileOptions
{
    /// <summary>
    /// Mode applied to every extension property that has no explicit
    /// <see cref="ProfileExtensionPropertyInfoExtensions.ConfigureProfile(Volo.Abp.ObjectExtending.ObjectExtensionPropertyInfo, System.Action{ProfileExtensionPropertyConfiguration})"/>
    /// call. Defaults to <see cref="ProfileExtensionPropertyMode.ReadOnly"/> because these
    /// fields are normally assigned by administrators when the user is created.
    /// </summary>
    public ProfileExtensionPropertyMode DefaultExtensionPropertyMode { get; set; } =
        ProfileExtensionPropertyMode.ReadOnly;
}
