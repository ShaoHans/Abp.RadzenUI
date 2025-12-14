using Volo.Abp.Data;

namespace Abp.RadzenUI.Components.ObjectExtending;

public partial class TimeExtensionProperty<TEntity, TResourceType>
    where TEntity : IHasExtraProperties
{
    protected TimeOnly? Value
    {
        get
        {
            return PropertyInfo.GetInputValueOrDefault<TimeOnly?>(
                Entity.GetProperty(PropertyInfo.Name)
            );
        }
        set { Entity.SetProperty(PropertyInfo.Name, value, validate: false); }
    }
}
