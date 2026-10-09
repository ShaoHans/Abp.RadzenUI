using Abp.RadzenUI.ObjectExtending;
using Volo.Abp.ObjectExtending;
using Volo.Abp.ObjectExtending.Modularity;
using Xunit;

namespace Abp.RadzenUI.Tests;

public class ModuleExtensionTableVisibilityTests
{
    // ObjectExtensionManager.Instance is a process-wide singleton; use a unique module name.
    private const string Module = "AbpRadzenUI.Tests.TableVisibility";
    private const string Entity = "User";

    static ModuleExtensionTableVisibilityTests()
    {
        ObjectExtensionManager.Instance.Modules().ConfigureModule<ModuleExtensionConfiguration>(Module, module =>
        {
            module.ConfigureEntity(Entity, entity =>
            {
                entity.AddOrUpdateProperty<string>("ShownColumn");
                entity.AddOrUpdateProperty<string>("HiddenColumn", p => p.UI.OnTable.IsVisible = false);
            });
        });
    }

    [Fact]
    public void Property_Hidden_On_Table_At_Module_Level_Should_Not_Be_Visible()
    {
        Assert.False(ModuleExtensionPropertyHelper.IsVisibleOnTable(Module, Entity, "HiddenColumn"));
    }

    [Fact]
    public void Property_Without_OnTable_Setting_Should_Be_Visible()
    {
        Assert.True(ModuleExtensionPropertyHelper.IsVisibleOnTable(Module, Entity, "ShownColumn"));
    }

    [Fact]
    public void Property_Not_Defined_At_Module_Level_Should_Be_Visible()
    {
        // e.g. defined directly on the DTO via AddOrUpdateProperty<IdentityUserDto, ...>
        Assert.True(ModuleExtensionPropertyHelper.IsVisibleOnTable(Module, Entity, "DtoOnlyColumn"));
        Assert.True(ModuleExtensionPropertyHelper.IsVisibleOnTable("NoSuchModule", Entity, "X"));
    }
}
