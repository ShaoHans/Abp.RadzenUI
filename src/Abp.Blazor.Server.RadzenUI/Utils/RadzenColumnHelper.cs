using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Radzen.Blazor;
using Volo.Abp.Data;
using Volo.Abp.ObjectExtending;

namespace Abp.RadzenUI.Utils;

public static class RadzenColumnHelper
{
    public static RenderFragment ExtraPropertiesColumns<TItem>(IStringLocalizer l)
        where TItem : class, IHasExtraProperties
    {
        return builder =>
        {
            var props = ObjectExtensionManager.Instance.GetProperties<TItem>();

            foreach (var prop in props)
            {
                builder.OpenComponent<RadzenDataGridColumn<TItem>>(0);

                // ----- Title -----
                var title = prop.Name;

                if (prop.Configuration != null)
                {
                    if (prop.Configuration.TryGetValue("LocalizationKey", out var lk))
                        title = l[lk.ToString()!];

                    if (prop.Configuration.TryGetValue("Title", out var t))
                        title = t.ToString()!;
                }

                builder.AddAttribute(1, "Title", title);
                builder.AddAttribute(2, "Sortable", false);
                builder.AddAttribute(3, "Filterable", false);

                if (prop.Configuration?.TryGetValue("Width", out var w) == true)
                {
                    builder.AddAttribute(4, "Width", w.ToString());
                }

                // ----- Template -----
                builder.AddAttribute(
                    5,
                    "Template",
                    (RenderFragment<TItem>)(
                        context =>
                            tb =>
                            {
                                if (context.ExtraProperties.TryGetValue(prop.Name, out var value))
                                {
                                    var text = value switch
                                    {
                                        DateTime dt => dt.ToString("yyyy-MM-dd"),
                                        _ => value?.ToString()
                                    };

                                    tb.AddContent(0, text);
                                }
                            }
                    )
                );

                builder.CloseComponent();
            }
        };
    }
}
