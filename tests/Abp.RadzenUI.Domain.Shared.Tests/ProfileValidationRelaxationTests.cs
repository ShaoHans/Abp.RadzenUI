using System.ComponentModel.DataAnnotations;
using Abp.RadzenUI.ObjectExtending;
using Volo.Abp.ObjectExtending;
using Xunit;

namespace Abp.RadzenUI.Tests;

/// <summary>
/// A user cannot fill a ReadOnly / Hidden property on the profile page, so validation
/// attributes that were copied from the module-level definition (e.g. Required) must not
/// block saving the profile. Editable properties keep their rules.
/// </summary>
public class ProfileValidationRelaxationTests
{
    private class FakeUpdateProfileDto : ExtensibleObject;

    private static ObjectExtensionPropertyInfo NewProperty(string name)
    {
        var property = new ObjectExtensionPropertyInfo(
            new ObjectExtensionInfo(typeof(FakeUpdateProfileDto)),
            typeof(string),
            name
        );
        property.Attributes.Add(new RequiredAttribute());
        property.Attributes.Add(new StringLengthAttribute(64));
        property.Attributes.Add(new DataTypeAttribute(DataType.Text));
        return property;
    }

    [Theory]
    [InlineData(ProfileExtensionPropertyMode.ReadOnly)]
    [InlineData(ProfileExtensionPropertyMode.Hidden)]
    public void Non_Editable_Property_Should_Lose_Validation_Attributes_But_Keep_DataType(
        ProfileExtensionPropertyMode mode
    )
    {
        var property = NewProperty("Department");
        property.ConfigureProfile(c => c.Mode = mode);

        ProfileExtensionPropertyHelper.RelaxValidationForNonEditableProperties(
            [property],
            new AbpRadzenUIProfileOptions()
        );

        Assert.DoesNotContain(property.Attributes, a => a is RequiredAttribute);
        Assert.DoesNotContain(property.Attributes, a => a is StringLengthAttribute);
        Assert.Contains(property.Attributes, a => a is DataTypeAttribute);
    }

    [Fact]
    public void Editable_Property_Should_Keep_Validation_Attributes()
    {
        var property = NewProperty("Nickname");
        property.ConfigureProfile(c => c.Mode = ProfileExtensionPropertyMode.Editable);

        ProfileExtensionPropertyHelper.RelaxValidationForNonEditableProperties(
            [property],
            new AbpRadzenUIProfileOptions()
        );

        Assert.Contains(property.Attributes, a => a is RequiredAttribute);
        Assert.Contains(property.Attributes, a => a is StringLengthAttribute);
    }

    [Fact]
    public void Unconfigured_Property_Should_Follow_Default_Mode()
    {
        var property = NewProperty("Department");

        ProfileExtensionPropertyHelper.RelaxValidationForNonEditableProperties(
            [property],
            new AbpRadzenUIProfileOptions
            {
                DefaultExtensionPropertyMode = ProfileExtensionPropertyMode.ReadOnly
            }
        );

        Assert.DoesNotContain(property.Attributes, a => a is RequiredAttribute);
    }
}
