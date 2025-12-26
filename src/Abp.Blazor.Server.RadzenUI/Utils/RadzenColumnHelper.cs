using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Radzen.Blazor;
using Volo.Abp.Data;
using Volo.Abp.ObjectExtending;

namespace Abp.RadzenUI.Utils;

public sealed class ExtraPropertyColumnMeta
{
    public string Name { get; init; } = default!;
    public string Title { get; init; } = default!;
    public string? Width { get; init; }

    public string? FormatString { get; set; }
}

public static class RadzenColumnHelper
{
    public static List<ExtraPropertyColumnMeta> GetExtraPropertyMetas<TItem>(IStringLocalizer l)
    {
        return
        [
            .. ObjectExtensionManager
                .Instance.GetProperties<TItem>()
                .Select(prop =>
                {
                    var title = prop.Name;

                    if (prop.Configuration != null)
                    {
                        if (prop.Configuration.TryGetValue("LocalizationKey", out var lk))
                            title = l[lk.ToString()!];

                        if (prop.Configuration.TryGetValue("Title", out var t))
                            title = t.ToString()!;
                    }

                    return new ExtraPropertyColumnMeta
                    {
                        Name = prop.Name,
                        Title = title,
                        Width =
                            prop.Configuration?.TryGetValue("Width", out var w) == true
                                ? w.ToString()
                                : null,
                        FormatString =
                            prop.Configuration?.TryGetValue("FormatString", out var fs) == true
                                ? fs.ToString()
                                : null
                    };
                })
        ];
    }

    public static RenderFragment ExtraPropertiesColumns<TItem>(
        IReadOnlyList<ExtraPropertyColumnMeta> metas
    )
        where TItem : class, IHasExtraProperties
    {
        return builder =>
        {
            foreach (var meta in metas)
            {
                builder.OpenComponent<RadzenDataGridColumn<TItem>>(0);

                builder.AddAttribute(1, "Title", meta.Title);
                builder.AddAttribute(2, "Sortable", false);
                builder.AddAttribute(3, "Filterable", false);

                if (!string.IsNullOrEmpty(meta.Width))
                {
                    builder.AddAttribute(4, "Width", meta.Width);
                }

                if (!string.IsNullOrEmpty(meta.FormatString))
                {
                    builder.AddAttribute(5, "FormatString", meta.FormatString);
                }

                builder.AddAttribute(
                    6,
                    "Template",
                    (RenderFragment<TItem>)(
                        context =>
                            tb =>
                            {
                                if (context.ExtraProperties.TryGetValue(meta.Name, out var value))
                                {
                                    tb.AddContent(0, value);
                                }
                            }
                    )
                );

                builder.CloseComponent();
            }
        };
    }
}
