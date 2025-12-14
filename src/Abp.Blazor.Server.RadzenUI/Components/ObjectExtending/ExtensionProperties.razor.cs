using System.Collections.Immutable;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Volo.Abp.AspNetCore.Components.Web;
using Volo.Abp.Data;
using Volo.Abp.ObjectExtending;

namespace Abp.RadzenUI.Components.ObjectExtending;

public partial class ExtensionProperties<TEntityType, TResourceType> : ComponentBase
    where TEntityType : IHasExtraProperties
{
    [Inject]
    public IStringLocalizerFactory StringLocalizerFactory { get; set; } = default!;

    [Parameter]
    public AbpBlazorMessageLocalizerHelper<TResourceType> LH { get; set; } = default!;

    [Parameter]
    public TEntityType Entity { get; set; } = default!;

    [Inject]
    public IServiceProvider ServiceProvider { get; set; } = default!;

    public ImmutableList<ObjectExtensionPropertyInfo> Properties { get; set; } = [];

    protected override async Task OnInitializedAsync()
    {
        Properties =
            await ObjectExtensionManager.Instance.GetPropertiesAndCheckPolicyAsync<TEntityType>(
                ServiceProvider
            );
    }

    private RenderFragment ExtensionPropertyRender(ObjectExtensionPropertyInfo propertyInfo) =>
        builder =>
        {
            var inputType = propertyInfo.GetInputType();
            builder.OpenComponent(
                0,
                inputType.MakeGenericType(typeof(TEntityType), typeof(TResourceType))
            );
            builder.AddAttribute(1, "PropertyInfo", propertyInfo);
            builder.AddAttribute(2, "Entity", Entity);
            builder.AddAttribute(3, "LH", LH);
            builder.CloseComponent();
        };
}
