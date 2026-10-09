using Abp.RadzenUI.ObjectExtending;
using Volo.Abp.Data;
using Volo.Abp.ObjectExtending;
using Volo.Abp.ObjectExtending.Modularity;
using Xunit;

namespace Abp.RadzenUI.Tests;

public class ProfileExtensionPropertyTests
{
    private static ObjectExtensionPropertyInfo NewProperty(string name = "Department")
    {
        var objectExtension = new ObjectExtensionInfo(typeof(FakeProfileDto));
        return new ObjectExtensionPropertyInfo(objectExtension, typeof(string), name);
    }

    [Fact]
    public void GetProfileMode_Should_Fall_Back_To_Default_When_Not_Configured()
    {
        var property = NewProperty();
        var options = new AbpRadzenUIProfileOptions
        {
            DefaultExtensionPropertyMode = ProfileExtensionPropertyMode.Editable
        };

        Assert.Equal(ProfileExtensionPropertyMode.Editable, property.GetProfileMode(options));
    }

    [Fact]
    public void Default_Mode_Should_Be_ReadOnly()
    {
        Assert.Equal(
            ProfileExtensionPropertyMode.ReadOnly,
            new AbpRadzenUIProfileOptions().DefaultExtensionPropertyMode
        );
    }

    [Theory]
    [InlineData(ProfileExtensionPropertyMode.Hidden)]
    [InlineData(ProfileExtensionPropertyMode.ReadOnly)]
    [InlineData(ProfileExtensionPropertyMode.Editable)]
    public void GetProfileMode_Should_Return_Explicit_Configuration(ProfileExtensionPropertyMode mode)
    {
        var property = NewProperty();
        property.ConfigureProfile(profile => profile.Mode = mode);

        var options = new AbpRadzenUIProfileOptions
        {
            DefaultExtensionPropertyMode = mode == ProfileExtensionPropertyMode.Hidden
                ? ProfileExtensionPropertyMode.Editable
                : ProfileExtensionPropertyMode.Hidden
        };

        Assert.Equal(mode, property.GetProfileMode(options));
    }

    [Fact]
    public void ConfigureProfile_On_Module_Property_Configuration_Should_Be_Readable_From_Property_Info()
    {
        // Host apps configure properties via ConfigureIdentity(...).ConfigureUser(...),
        // which hands out an ExtensionPropertyConfiguration whose Configuration dictionary
        // ABP later copies onto every mapped DTO's ObjectExtensionPropertyInfo.
        var moduleConfig = new ExtensionPropertyConfiguration(
            new EntityExtensionConfiguration(),
            typeof(string),
            "Department"
        );
        moduleConfig.ConfigureProfile(profile => profile.Mode = ProfileExtensionPropertyMode.Editable);

        var property = NewProperty();
        foreach (var (key, value) in moduleConfig.Configuration)
        {
            property.Configuration[key] = value;
        }

        Assert.Equal(
            ProfileExtensionPropertyMode.Editable,
            property.GetProfileMode(new AbpRadzenUIProfileOptions())
        );
    }

    [Fact]
    public void RemoveNonEditableProperties_Should_Keep_Only_Editable_Properties()
    {
        var editable = NewProperty("Nickname");
        editable.ConfigureProfile(profile => profile.Mode = ProfileExtensionPropertyMode.Editable);
        var readOnly = NewProperty("EmployeeNo");
        readOnly.ConfigureProfile(profile => profile.Mode = ProfileExtensionPropertyMode.ReadOnly);
        var hidden = NewProperty("Salary");
        hidden.ConfigureProfile(profile => profile.Mode = ProfileExtensionPropertyMode.Hidden);
        var unconfigured = NewProperty("Department");

        var input = new FakeProfileDto();
        input.SetProperty("Nickname", "hans", validate: false);
        input.SetProperty("EmployeeNo", "E-1", validate: false);
        input.SetProperty("Salary", 1, validate: false);
        input.SetProperty("Department", "IT", validate: false);
        input.SetProperty("Undefined", "x", validate: false);

        ProfileExtensionPropertyHelper.RemoveNonEditableProperties(
            input,
            [editable, readOnly, hidden, unconfigured],
            new AbpRadzenUIProfileOptions()
        );

        Assert.True(input.HasProperty("Nickname"));
        Assert.False(input.HasProperty("EmployeeNo"));
        Assert.False(input.HasProperty("Salary"));
        Assert.False(input.HasProperty("Department"));
        // Properties without a definition are left to ABP's own mapping checks.
        Assert.True(input.HasProperty("Undefined"));
    }

    private class FakeProfileDto : ExtensibleObject;
}
