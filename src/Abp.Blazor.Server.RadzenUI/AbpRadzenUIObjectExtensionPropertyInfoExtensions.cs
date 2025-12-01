using System.ComponentModel.DataAnnotations;
using Volo.Abp.ObjectExtending;
using Volo.Abp.Reflection;

namespace Abp.RadzenUI;

public static class AbpRadzenUIObjectExtensionPropertyInfoExtensions
{
    public static bool IsRequired(this ObjectExtensionPropertyInfo propertyInfo)
    {
        if (propertyInfo.Attributes.Exists(attribute => attribute is RequiredAttribute))
        {
            return true;
        }
        return false;
    }

    public static string? GetTextInputValueOrNull(this IBasicObjectExtensionPropertyInfo property, object? value)
    {
        if (value == null)
        {
            return null;
        }

        if (TypeHelper.IsFloatingType(property.Type))
        {
            return value.ToString()?.Replace(',', '.');
        }

        return value.ToString();
    }
}
