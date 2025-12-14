using Volo.Abp.Data;

namespace Abp.RadzenUI.Components.ObjectExtending;

public partial class PasswordExtensionProperty<TEntity, TResourceType>
    where TEntity : IHasExtraProperties
{
    protected string? Value
    {
        get { return PropertyInfo.GetTextInputValueOrNull(Entity.GetProperty(PropertyInfo.Name)); }
        set { Entity.SetProperty(PropertyInfo.Name, value, validate: false); }
    }
}
