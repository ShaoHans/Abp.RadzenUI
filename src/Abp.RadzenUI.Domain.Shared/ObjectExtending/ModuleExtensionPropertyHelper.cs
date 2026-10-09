using Volo.Abp.ObjectExtending;
using Volo.Abp.ObjectExtending.Modularity;

namespace Abp.RadzenUI.ObjectExtending;

public static class ModuleExtensionPropertyHelper
{
    /// <summary>
    /// Reads <c>property.UI.OnTable.IsVisible</c> from the module-level extension
    /// configuration (<c>ObjectExtensionManager.Instance.Modules()</c>). ABP copies the
    /// create/edit form settings onto the DTO-level <see cref="ObjectExtensionPropertyInfo"/>
    /// but <b>not</b> the table setting, so list pages have to look it up here.
    /// Properties that are not defined at module level (e.g. added directly to a DTO)
    /// are treated as visible.
    /// </summary>
    public static bool IsVisibleOnTable(string moduleName, string entityName, string propertyName)
    {
        var property = ObjectExtensionManager
            .Instance.Modules()
            .GetOrDefault(moduleName)
            ?.Entities.GetOrDefault(entityName)
            ?.GetProperties().FirstOrDefault(p => p.Name == propertyName);

        return property?.UI.OnTable.IsVisible ?? true;
    }
}
