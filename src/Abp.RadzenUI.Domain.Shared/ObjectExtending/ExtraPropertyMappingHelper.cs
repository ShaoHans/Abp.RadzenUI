using Volo.Abp.Data;
using Volo.Abp.ObjectExtending;

namespace Abp.RadzenUI.ObjectExtending;

public static class ExtraPropertyMappingHelper
{
    /// <summary>
    /// Copies extra properties from a "get" DTO into an "update" DTO when opening an edit form.
    /// Uses ABP's own pair-definition rules (<see cref="ExtensibleObjectMapper.CanMapProperty"/>)
    /// but writes with <c>validate: false</c>: ABP's <c>MapExtraPropertiesTo</c> validates on every
    /// <c>SetProperty</c>, so a <c>[Required]</c> extension property that is still empty on an old
    /// record would throw before the user even sees the form. Validation belongs to the form and
    /// to the server on save, not to loading.
    /// </summary>
    public static void CopyExtraPropertiesForEditing<TSource, TDestination>(
        TSource source,
        TDestination destination
    )
        where TSource : IHasExtraProperties
        where TDestination : IHasExtraProperties
    {
        foreach (var (name, value) in source.ExtraProperties)
        {
            if (ExtensibleObjectMapper.CanMapProperty<TSource, TDestination>(name))
            {
                destination.SetProperty(name, value, validate: false);
            }
        }
    }
}
