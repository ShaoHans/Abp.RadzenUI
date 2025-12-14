using Volo.Abp.Data;

namespace Abp.RadzenUI.Components.ObjectExtending;

public partial class DateExtensionProperty<TEntity, TResourceType>
    where TEntity : IHasExtraProperties
{
    protected DateOnly? Value
    {
        get
        {
            return PropertyInfo.GetInputValueOrDefault<DateOnly?>(
                Entity.GetProperty(PropertyInfo.Name)
            );
        }
        set { Entity.SetProperty(PropertyInfo.Name, value, validate: false); }
    }
}
