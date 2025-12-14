using Volo.Abp.Data;

namespace Abp.RadzenUI.Components.ObjectExtending;

public partial class CheckBoxExtensionProperty<TEntity, TResourceType>
    where TEntity : IHasExtraProperties
{
    protected bool Value
    {
        get { return PropertyInfo.GetInputValueOrDefault<bool>(Entity.GetProperty(PropertyInfo.Name)); }
        set { Entity.SetProperty(PropertyInfo.Name, value, validate: false); }
    }
}
