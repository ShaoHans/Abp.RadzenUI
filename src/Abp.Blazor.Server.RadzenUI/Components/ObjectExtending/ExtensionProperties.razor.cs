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

    [Parameter]
    public IReadOnlyCollection<string> ExcludedPropertyNames { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Type whose extension property definitions are rendered. Defaults to
    /// <typeparamref name="TEntityType"/>. Set it when the bound object is a view model
    /// (e.g. <c>PersonalInfoModel</c>) that mirrors a DTO (e.g. <c>UpdateProfileDto</c>)
    /// carrying the actual definitions.
    /// </summary>
    [Parameter]
    public Type? DefinitionType { get; set; }

    /// <summary>
    /// Returns <c>true</c> for properties that must not be rendered at all.
    /// </summary>
    [Parameter]
    public Func<ObjectExtensionPropertyInfo, bool>? HiddenWhen { get; set; }

    /// <summary>
    /// Returns <c>true</c> for properties that are rendered read-only.
    /// </summary>
    [Parameter]
    public Func<ObjectExtensionPropertyInfo, bool>? ReadOnlyWhen { get; set; }

    [Parameter]
    public int LabelSizeMD { get; set; } = 4;

    [Parameter]
    public int InputSizeMD { get; set; } = 8;

    /// <summary>
    /// Inline style for every rendered input. Defaults to filling the column.
    /// </summary>
    [Parameter]
    public string InputStyle { get; set; } = "display: block; width: 100%;";

    [Inject]
    public IServiceProvider ServiceProvider { get; set; } = default!;

    public ImmutableList<ObjectExtensionPropertyInfo> Properties { get; set; } = [];

    protected override async Task OnInitializedAsync()
    {
        Properties = await ObjectExtensionManager.Instance.GetPropertiesAndCheckPolicyAsync(
            DefinitionType ?? typeof(TEntityType),
            ServiceProvider
        );
    }

    private bool IsHidden(ObjectExtensionPropertyInfo propertyInfo) =>
        propertyInfo.Name.EndsWith("_Text")
        || ExcludedPropertyNames.Contains(propertyInfo.Name)
        || (HiddenWhen?.Invoke(propertyInfo) ?? false);

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
            builder.AddAttribute(4, "ReadOnly", ReadOnlyWhen?.Invoke(propertyInfo) ?? false);
            builder.AddAttribute(5, "LabelSizeMD", LabelSizeMD);
            builder.AddAttribute(6, "InputSizeMD", InputSizeMD);
            builder.AddAttribute(7, "InputStyle", InputStyle);
            builder.CloseComponent();
        };
}
