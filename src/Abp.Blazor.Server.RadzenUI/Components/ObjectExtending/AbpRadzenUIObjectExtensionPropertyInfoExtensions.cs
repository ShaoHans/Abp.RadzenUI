using System.ComponentModel.DataAnnotations;
using Volo.Abp.ObjectExtending;
using Volo.Abp.Reflection;

namespace Abp.RadzenUI.Components.ObjectExtending;

public static class AbpRadzenUIObjectExtensionPropertyInfoExtensions
{
    private static readonly HashSet<Type> NumberTypes =
    [
        typeof(int),
        typeof(long),
        typeof(byte),
        typeof(sbyte),
        typeof(short),
        typeof(ushort),
        typeof(uint),
        typeof(long),
        typeof(ulong),
        typeof(float),
        typeof(double),
        typeof(decimal),
        typeof(int?),
        typeof(long?),
        typeof(byte?),
        typeof(sbyte?),
        typeof(short?),
        typeof(ushort?),
        typeof(uint?),
        typeof(long?),
        typeof(ulong?),
        typeof(float?),
        typeof(double?),
        typeof(decimal?)
    ];

    private static readonly HashSet<Type> TextEditSupportedAttributeTypes =
    [
        typeof(EmailAddressAttribute),
        typeof(UrlAttribute),
        typeof(PhoneAttribute)
    ];

    public static bool IsRequired(this ObjectExtensionPropertyInfo propertyInfo)
    {
        if (propertyInfo.Attributes.Exists(attribute => attribute is RequiredAttribute))
        {
            return true;
        }
        return false;
    }

    public static string? GetTextInputValueOrNull(
        this IBasicObjectExtensionPropertyInfo property,
        object? value
    )
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

    public static T? GetInputValueOrDefault<T>(
        this IBasicObjectExtensionPropertyInfo property,
        object? value
    )
    {
        if (value == null)
        {
            return default;
        }

        return (T)value;
    }

    public static Type GetInputType(this ObjectExtensionPropertyInfo propertyInfo)
    {
        foreach (var attribute in propertyInfo.Attributes)
        {
            var inputTypeByAttribute = GetInputTypeFromAttributeOrNull(attribute);
            if (inputTypeByAttribute != null)
            {
                return inputTypeByAttribute;
            }
        }
        return GetInputTypeFromTypeOrNull(propertyInfo.Type) ?? typeof(TextBoxExtensionProperty<,>); //default
    }

    private static Type? GetInputTypeFromAttributeOrNull(Attribute attribute)
    {
        var hasTextEditSupport = TextEditSupportedAttributeTypes.Any(t => t == attribute.GetType());

        if (hasTextEditSupport)
        {
            return typeof(TextBoxExtensionProperty<,>);
        }

        if (attribute is DataTypeAttribute dataTypeAttribute)
        {
            switch (dataTypeAttribute.DataType)
            {
                case DataType.Password:
                    return typeof(PasswordExtensionProperty<,>);
                case DataType.DateTime:
                    return typeof(DateTimeExtensionProperty<,>);
                case DataType.Date:
                    return typeof(DateExtensionProperty<,>);
                case DataType.Time:
                    return typeof(TimeExtensionProperty<,>);
                case DataType.EmailAddress:
                    return typeof(EmailExtensionProperty<,>);
                //case DataType.Url:
                //    return typeof(TextExtensionProperty<,>);
                //case DataType.PhoneNumber:
                //    return typeof(TextExtensionProperty<,>);
            }
        }

        return null;
    }

    private static Type? GetInputTypeFromTypeOrNull(Type type)
    {
        if (type == typeof(bool))
        {
            return typeof(CheckBoxExtensionProperty<,>);
        }

        if (type == typeof(DateTime))
        {
            return typeof(DateTimeExtensionProperty<,>);
        }

        if (NumberTypes.Contains(type))
        {
            return typeof(NumberExtensionProperty<,>);
        }

        return null;
    }
}
