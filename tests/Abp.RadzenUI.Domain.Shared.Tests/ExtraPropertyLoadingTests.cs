using System.ComponentModel.DataAnnotations;
using Abp.RadzenUI.ObjectExtending;
using Volo.Abp.Data;
using Volo.Abp.ObjectExtending;
using Xunit;

namespace Abp.RadzenUI.Tests;

/// <summary>
/// Loading an existing record into an edit form must never throw because of validation
/// rules; validation belongs to the form and to the server on save.
/// </summary>
public class ExtraPropertyLoadingTests
{
    private class SourceDto : ExtensibleObject;

    private class TargetDto : ExtensibleObject;

    static ExtraPropertyLoadingTests()
    {
        ObjectExtensionManager.Instance.AddOrUpdateProperty<SourceDto, string>(
            "Department",
            p => p.CheckPairDefinitionOnMapping = false
        );
        ObjectExtensionManager.Instance.AddOrUpdateProperty<TargetDto, string>(
            "Department",
            p =>
            {
                p.Attributes.Add(new RequiredAttribute());
                p.CheckPairDefinitionOnMapping = false;
            }
        );
        ObjectExtensionManager.Instance.AddOrUpdateProperty<TargetDto, string>(
            "TargetOnly",
            p => p.CheckPairDefinitionOnMapping = true
        );
    }

    [Fact]
    public void Abp_Default_Mapping_Throws_For_Required_Property_With_Null_Value()
    {
        // Documents the behavior we are working around.
        var source = new SourceDto();
        source.SetProperty("Department", null, validate: false);

        Assert.ThrowsAny<Exception>(() => source.MapExtraPropertiesTo(new TargetDto()));
    }

    [Fact]
    public void CopyExtraPropertiesForEditing_Should_Not_Validate()
    {
        var source = new SourceDto();
        source.SetProperty("Department", null, validate: false);
        var target = new TargetDto();

        ExtraPropertyMappingHelper.CopyExtraPropertiesForEditing(source, target);

        Assert.True(target.HasProperty("Department"));
        Assert.Null(target.GetProperty("Department"));
    }

    [Fact]
    public void CopyExtraPropertiesForEditing_Should_Copy_Values()
    {
        var source = new SourceDto();
        source.SetProperty("Department", "IT", validate: false);
        var target = new TargetDto();

        ExtraPropertyMappingHelper.CopyExtraPropertiesForEditing(source, target);

        Assert.Equal("IT", target.GetProperty<string>("Department"));
    }

    [Fact]
    public void CopyExtraPropertiesForEditing_Should_Map_The_Same_Keys_As_Abp()
    {
        // The helper only differs from ABP's MapExtraPropertiesTo by skipping validation,
        // so with valid values both must produce the same set of keys.
        var source = new SourceDto();
        source.SetProperty("Department", "IT", validate: false);
        source.SetProperty("Undefined", 1, validate: false);
        source.SetProperty("TargetOnly", "x", validate: false);

        var viaAbp = new TargetDto();
        source.MapExtraPropertiesTo(viaAbp);

        var viaHelper = new TargetDto();
        ExtraPropertyMappingHelper.CopyExtraPropertiesForEditing(source, viaHelper);

        Assert.Equal(
            viaAbp.ExtraProperties.Keys.Order(),
            viaHelper.ExtraProperties.Keys.Order()
        );
    }
}
