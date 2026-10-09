using Volo.Abp.ObjectExtending;
using Volo.Abp.ObjectExtending.Modularity;

namespace Abp.RadzenUI.ObjectExtending;

public static class ProfileExtensionPropertyInfoExtensions
{
    /// <summary>
    /// Key used inside ABP's <c>Configuration</c> dictionaries. ABP copies that
    /// dictionary from the module-level <see cref="ExtensionPropertyConfiguration"/>
    /// onto every mapped DTO's <see cref="ObjectExtensionPropertyInfo"/>.
    /// </summary>
    public const string ConfigurationKey = "AbpRadzenUI.Profile";

    /// <summary>
    /// Configures profile-page behavior when defining a property through
    /// <c>ObjectExtensionManager.Instance.Modules().ConfigureIdentity(i => i.ConfigureUser(...))</c>.
    /// </summary>
    public static ExtensionPropertyConfiguration ConfigureProfile(
        this ExtensionPropertyConfiguration property,
        Action<ProfileExtensionPropertyConfiguration> configure
    )
    {
        configure(GetOrCreate(property.Configuration));
        return property;
    }

    /// <summary>
    /// Configures profile-page behavior when defining a property directly on a DTO type
    /// through <c>ObjectExtensionManager.Instance.AddOrUpdateProperty&lt;TDto, TProperty&gt;(...)</c>.
    /// </summary>
    public static ObjectExtensionPropertyInfo ConfigureProfile(
        this ObjectExtensionPropertyInfo property,
        Action<ProfileExtensionPropertyConfiguration> configure
    )
    {
        configure(GetOrCreate(property.Configuration));
        return property;
    }

    public static ProfileExtensionPropertyConfiguration? GetProfileConfigurationOrNull(
        this ObjectExtensionPropertyInfo property
    )
    {
        return property.Configuration.TryGetValue(ConfigurationKey, out var value)
            ? value as ProfileExtensionPropertyConfiguration
            : null;
    }

    /// <summary>
    /// Resolves the effective mode: the explicit per-property setting when present,
    /// otherwise <see cref="AbpRadzenUIProfileOptions.DefaultExtensionPropertyMode"/>.
    /// </summary>
    public static ProfileExtensionPropertyMode GetProfileMode(
        this ObjectExtensionPropertyInfo property,
        AbpRadzenUIProfileOptions options
    )
    {
        return property.GetProfileConfigurationOrNull()?.Mode
            ?? options.DefaultExtensionPropertyMode;
    }

    // ExtensionPropertyConfiguration.Configuration is Dictionary<string, object> while
    // ObjectExtensionPropertyInfo.Configuration is Dictionary<object, object>.
    private static ProfileExtensionPropertyConfiguration GetOrCreate<TKey>(
        IDictionary<TKey, object> configuration
    )
        where TKey : notnull
    {
        var key = (TKey)(object)ConfigurationKey;
        if (
            configuration.TryGetValue(key, out var existing)
            && existing is ProfileExtensionPropertyConfiguration typed
        )
        {
            return typed;
        }

        var created = new ProfileExtensionPropertyConfiguration();
        configuration[key] = created;
        return created;
    }
}
