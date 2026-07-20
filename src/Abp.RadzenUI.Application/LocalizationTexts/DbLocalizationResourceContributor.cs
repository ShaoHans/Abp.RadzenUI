using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Volo.Abp.Localization;

namespace Abp.RadzenUI.Application.LocalizationTexts;

/// <summary>
/// A dynamic localization resource contributor that overlays database overrides on top of the
/// static (JSON) baseline. It is added <b>after</b> the static contributor so that, per
/// <see cref="LocalizationResourceContributorList"/> semantics (last contributor wins), database
/// values take precedence over the JSON baseline.
/// </summary>
public class DbLocalizationResourceContributor : ILocalizationResourceContributor
{
    public bool IsDynamic => true;

    private string _resourceName = default!;
    private IServiceProvider _serviceProvider = default!;
    private LocalizationTextStore? _store;

    public void Initialize(LocalizationResourceInitializationContext context)
    {
        _resourceName = context.Resource.ResourceName;
        _serviceProvider = context.ServiceProvider;
    }

    public LocalizedString? GetOrNull(string cultureName, string name)
    {
        var texts = Store?.PeekOrNull(Store.CurrentTenantId, _resourceName, cultureName);
        if (texts != null && texts.TryGetValue(name, out var value))
        {
            return new LocalizedString(name, value, resourceNotFound: false, searchedLocation: _resourceName);
        }

        return null;
    }

    public void Fill(string cultureName, Dictionary<string, LocalizedString> dictionary)
    {
        var texts = Store?.PeekOrNull(Store.CurrentTenantId, _resourceName, cultureName);
        if (texts == null)
        {
            return;
        }

        foreach (var kv in texts)
        {
            dictionary[kv.Key] = new LocalizedString(kv.Key, kv.Value, resourceNotFound: false, searchedLocation: _resourceName);
        }
    }

    public async Task FillAsync(string cultureName, Dictionary<string, LocalizedString> dictionary)
    {
        if (Store == null)
        {
            return;
        }

        var texts = await Store.GetAsync(Store.CurrentTenantId, _resourceName, cultureName);
        foreach (var kv in texts)
        {
            dictionary[kv.Key] = new LocalizedString(kv.Key, kv.Value, resourceNotFound: false, searchedLocation: _resourceName);
        }
    }

    public Task<IEnumerable<string>> GetSupportedCulturesAsync()
    {
        // Overrides target existing (statically declared) cultures, whose culture list is already
        // supplied by the static contributor. Returning empty keeps this contributor from forcing
        // extra database reads while still overlaying values via GetOrNull/Fill.
        return Task.FromResult(Enumerable.Empty<string>());
    }

    private LocalizationTextStore? Store =>
        _store ??= _serviceProvider?.GetService<LocalizationTextStore>();
}
