using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.RadzenUI.Application.Contracts.LocalizationTexts;
using Abp.RadzenUI.Localization;
using Abp.RadzenUI.LocalizationTexts;
using Abp.RadzenUI.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Localization;
using Volo.Abp.Uow;

namespace Abp.RadzenUI.Application.LocalizationTexts;

[Authorize(RadzenUIPermissions.Localization.Default)]
public class LocalizationTextAppService : ApplicationService, ILocalizationTextAppService
{
    private readonly IRepository<LocalizationText, Guid> _repository;
    private readonly LocalizationTextStore _store;
    private readonly IStringLocalizerFactory _stringLocalizerFactory;
    private readonly IUnitOfWorkManager _unitOfWorkManager;
    private readonly AbpLocalizationOptions _localizationOptions;

    public LocalizationTextAppService(
        IRepository<LocalizationText, Guid> repository,
        LocalizationTextStore store,
        IStringLocalizerFactory stringLocalizerFactory,
        IUnitOfWorkManager unitOfWorkManager,
        IOptions<AbpLocalizationOptions> localizationOptions)
    {
        _repository = repository;
        _store = store;
        _stringLocalizerFactory = stringLocalizerFactory;
        _unitOfWorkManager = unitOfWorkManager;
        _localizationOptions = localizationOptions.Value;
        LocalizationResource = typeof(AbpRadzenUIResource);
    }

    public Task<ListResultDto<LocalizationResourceInfoDto>> GetResourcesAsync()
    {
        var resources = _localizationOptions.Resources.Values
            .Select(r => r.ResourceName)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Distinct()
            .OrderBy(name => name)
            .Select(name => new LocalizationResourceInfoDto { Name = name })
            .ToList();

        return Task.FromResult(new ListResultDto<LocalizationResourceInfoDto>(resources));
    }

    public Task<ListResultDto<LocalizationCultureDto>> GetCulturesAsync()
    {
        var cultures = _localizationOptions.Languages
            .Select(l => new LocalizationCultureDto
            {
                CultureName = l.CultureName,
                DisplayName = l.DisplayName,
            })
            .ToList();

        return Task.FromResult(new ListResultDto<LocalizationCultureDto>(cultures));
    }

    public async Task<PagedResultDto<LocalizationTextItemDto>> GetListAsync(GetLocalizationTextsInput input)
    {
        var resource = GetResourceOrThrow(input.ResourceName);

        var baseDict = GetBaselineTexts(resource, input.CultureName);

        var overrides = await _repository.GetListAsync(x =>
            x.ResourceName == input.ResourceName && x.CultureName == input.CultureName);
        var overrideMap = overrides.ToDictionary(x => x.Key, x => x);

        var keys = new HashSet<string>(baseDict.Keys);
        foreach (var key in overrideMap.Keys)
        {
            keys.Add(key);
        }

        var items = keys.Select(key =>
        {
            overrideMap.TryGetValue(key, out var ov);
            baseDict.TryGetValue(key, out var baseValue);

            return new LocalizationTextItemDto
            {
                Id = ov?.Id,
                ResourceName = input.ResourceName,
                CultureName = input.CultureName,
                Key = key,
                BaseValue = baseValue,
                OverrideValue = ov?.Value,
            };
        });

        if (input.OnlyOverridden)
        {
            items = items.Where(x => x.Id.HasValue);
        }

        if (!string.IsNullOrWhiteSpace(input.Filter))
        {
            var filter = input.Filter.Trim();
            items = items.Where(x =>
                x.Key.Contains(filter, StringComparison.OrdinalIgnoreCase)
                || (x.BaseValue?.Contains(filter, StringComparison.OrdinalIgnoreCase) ?? false)
                || (x.OverrideValue?.Contains(filter, StringComparison.OrdinalIgnoreCase) ?? false));
        }

        var ordered = items.OrderBy(x => x.Key, StringComparer.OrdinalIgnoreCase).ToList();
        var page = ordered
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount)
            .ToList();

        return new PagedResultDto<LocalizationTextItemDto>(ordered.Count, page);
    }

    [Authorize(RadzenUIPermissions.Localization.Edit)]
    public async Task SaveAsync(SaveLocalizationTextDto input)
    {
        GetResourceOrThrow(input.ResourceName);
        input.Key = input.Key.Trim();

        var existing = await _repository.FirstOrDefaultAsync(x =>
            x.ResourceName == input.ResourceName
            && x.CultureName == input.CultureName
            && x.Key == input.Key);

        if (existing == null)
        {
            await _repository.InsertAsync(
                new LocalizationText
                {
                    ResourceName = input.ResourceName,
                    CultureName = input.CultureName,
                    Key = input.Key,
                    Value = input.Value,
                },
                autoSave: true);
        }
        else
        {
            existing.Value = input.Value;
            await _repository.UpdateAsync(existing, autoSave: true);
        }

        InvalidateCacheAfterCompleted(input.ResourceName, input.CultureName);
    }

    [Authorize(RadzenUIPermissions.Localization.Delete)]
    public async Task ResetAsync(ResetLocalizationTextDto input)
    {
        var existing = await _repository.FirstOrDefaultAsync(x =>
            x.ResourceName == input.ResourceName
            && x.CultureName == input.CultureName
            && x.Key == input.Key);

        if (existing == null)
        {
            return;
        }

        await _repository.DeleteAsync(existing, autoSave: true);
        InvalidateCacheAfterCompleted(input.ResourceName, input.CultureName);
    }

    /// <summary>
    /// Refreshes the localization cache only after the current unit of work commits. Reloading
    /// inside the UoW would open a separate transaction that cannot see the pending change, warming
    /// the cache with stale data.
    /// </summary>
    private void InvalidateCacheAfterCompleted(string resourceName, string cultureName)
    {
        var tenantId = CurrentTenant.Id;

        if (_unitOfWorkManager.Current != null)
        {
            _unitOfWorkManager.Current.OnCompleted(() => _store.InvalidateAsync(tenantId, resourceName, cultureName));
        }
        else
        {
            _ = _store.InvalidateAsync(tenantId, resourceName, cultureName);
        }
    }

    private LocalizationResourceBase GetResourceOrThrow(string resourceName)
    {
        var resource = _localizationOptions.Resources.Values
            .FirstOrDefault(r => r.ResourceName == resourceName);

        if (resource == null)
        {
            throw new BusinessException(LocalizationTextErrorCodes.ResourceNotFound)
                .WithData("name", resourceName);
        }

        return resource;
    }

    /// <summary>
    /// Returns the static (JSON) baseline texts of a resource/culture, excluding dynamic (database)
    /// contributors. Resolving the localizer first ensures the resource contributors are initialized.
    /// </summary>
    private Dictionary<string, string> GetBaselineTexts(LocalizationResourceBase resource, string cultureName)
    {
        // Resolving the localizer forces the resource contributors to initialize (loads JSON files).
        var localizer = _stringLocalizerFactory.CreateByResourceNameOrNull(resource.ResourceName);
        _ = localizer?.GetAllStrings(includeParentCultures: false).ToList();

        var filled = new Dictionary<string, LocalizedString>();
        resource.Contributors.Fill(cultureName, filled, includeDynamicContributors: false);

        return filled.ToDictionary(kv => kv.Key, kv => kv.Value.Value);
    }
}
