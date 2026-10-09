using System.ComponentModel.DataAnnotations;
using Volo.Abp.Data;
using Volo.Abp.ObjectExtending;

namespace Abp.RadzenUI.ObjectExtending;

public static class ProfileExtensionPropertyHelper
{
    /// <summary>
    /// Removes every defined extension property that is not
    /// <see cref="ProfileExtensionPropertyMode.Editable"/> from <paramref name="input"/>,
    /// so ABP's <c>MapExtraPropertiesTo</c> leaves the stored value untouched.
    /// Properties without a definition are left alone; ABP's own mapping checks handle them.
    /// </summary>
    public static void RemoveNonEditableProperties(
        IHasExtraProperties input,
        IEnumerable<ObjectExtensionPropertyInfo> properties,
        AbpRadzenUIProfileOptions options
    )
    {
        foreach (var property in properties)
        {
            if (property.GetProfileMode(options) != ProfileExtensionPropertyMode.Editable)
            {
                input.RemoveProperty(property.Name);
            }
        }
    }

    /// <summary>
    /// Strips validation rules from properties the user cannot edit on the profile page.
    /// Module-level definitions copy attributes such as <c>[Required]</c> onto
    /// <c>UpdateProfileDto</c>; since a ReadOnly / Hidden property cannot be filled there,
    /// those rules would otherwise block saving the profile for users whose value is still empty.
    /// <see cref="DataTypeAttribute"/> is kept because it only selects the input type.
    /// </summary>
    public static void RelaxValidationForNonEditableProperties(
        IEnumerable<ObjectExtensionPropertyInfo> properties,
        AbpRadzenUIProfileOptions options
    )
    {
        foreach (var property in properties)
        {
            if (property.GetProfileMode(options) == ProfileExtensionPropertyMode.Editable)
            {
                continue;
            }

            property.Attributes.RemoveAll(a => a is ValidationAttribute and not DataTypeAttribute);
            property.Validators.Clear();
        }
    }
}
