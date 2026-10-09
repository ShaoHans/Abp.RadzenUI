using System.Globalization;
using Abp.RadzenUI.ObjectExtending;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Radzen.Blazor;
using Volo.Abp.Data;
using Volo.Abp.ObjectExtending;

namespace Abp.RadzenUI.Infrastructure.Utils;

public sealed class ExtraPropertyColumnMeta
{
    public string Name { get; init; } = default!;
    public string LocalizationKey { get; init; } = default!;
    public string? Title { get; init; }
    public string? Width { get; init; }

    public string? FormatString { get; init; }
}

public static class RadzenColumnHelper
{
    public static List<ExtraPropertyColumnMeta> GetExtraPropertyMetas<TItem>()
    {
        return GetExtraPropertyMetas<TItem>(null, null, null);
    }

    /// <summary>
    /// Builds column metadata for the extension properties of <typeparamref name="TItem"/>.
    /// When <paramref name="moduleName"/> / <paramref name="entityName"/> are given, properties
    /// configured with <c>UI.OnTable.IsVisible = false</c> at module level are skipped
    /// (ABP does not copy that flag onto the DTO-level property info). The column title falls back to
    /// the property's <c>DisplayName</c> when no <c>Title</c> / <c>LocalizationKey</c> configuration exists.
    /// </summary>
    public static List<ExtraPropertyColumnMeta> GetExtraPropertyMetas<TItem>(
        string? moduleName,
        string? entityName,
        IStringLocalizerFactory? stringLocalizerFactory
    )
    {
        return
        [
            .. ObjectExtensionManager
                .Instance.GetProperties<TItem>()
                .Where(prop =>
                    moduleName == null
                    || entityName == null
                    || ModuleExtensionPropertyHelper.IsVisibleOnTable(moduleName, entityName, prop.Name)
                )
                .Select(prop =>
                {
                    var key =
                        prop.Configuration?.TryGetValue("LocalizationKey", out var lk) == true
                            ? lk.ToString()!
                            : $"DisplayName:{typeof(TItem).Name}.{prop.Name}";

                    var title =
                        prop.Configuration?.TryGetValue("Title", out var t) == true
                            ? t.ToString()
                            : null;

                    if (
                        title == null
                        && prop.DisplayName != null
                        && stringLocalizerFactory != null
                        && prop.Configuration?.ContainsKey("LocalizationKey") != true
                    )
                    {
                        title = prop.DisplayName.Localize(stringLocalizerFactory);
                    }

                    return new ExtraPropertyColumnMeta
                    {
                        Name = prop.Name,
                        LocalizationKey = key,
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
        IReadOnlyList<ExtraPropertyColumnMeta> metas,
        IStringLocalizerFactory stringLocalizerFactory
    )
        where TItem : class, IHasExtraProperties
    {
        return builder =>
        {
            foreach (var meta in metas)
            {
                builder.OpenComponent<RadzenDataGridColumn<TItem>>(0);

                builder.AddAttribute(
                    1,
                    "Title",
                    meta.Title
                        ?? UiLocalizationHelper.GetDisplayName(
                            meta.Name,
                            meta.LocalizationKey,
                            stringLocalizerFactory
                        )
                );
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
                                if (
                                    context.ExtraProperties.TryGetValue(meta.Name, out var value)
                                    && value != null
                                )
                                {
                                    string text;

                                    if (!string.IsNullOrEmpty(meta.FormatString))
                                    {
                                        try
                                        {
                                            text = string.Format(
                                                CultureInfo.CurrentUICulture,
                                                meta.FormatString,
                                                value
                                            );
                                        }
                                        catch
                                        {
                                            text = value.ToString()!;
                                        }
                                    }
                                    else
                                    {
                                        text = value.ToString()!;
                                    }

                                    tb.AddContent(0, text);
                                }
                                else
                                {
                                    tb.AddContent(0, string.Empty);
                                }
                            }
                    )
                );

                builder.CloseComponent();
            }
        };
    }
}
