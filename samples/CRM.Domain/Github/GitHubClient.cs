using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CRM.Github.Dtos;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;

namespace CRM.Github;

public class GitHubClient(IHttpClientFactory httpClientFactory, ILogger<GitHubClient> logger)
    : ITransientDependency
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("GitHub");

    public async Task<RepositoryDto> GetRepositoryAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<RepositoryDto>("/repos/ShaoHans/Abp.RadzenUI")
                ?? new();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "get repos info occur exception");
            return new();
        }
    }

    public async Task<List<RepositoryStargazerDto>> GetRepositoryStargazersAsync(
        int pageSize = 20,
        int pageNumber = 1
    )
    {
        try
        {
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "application/vnd.github.star+json");
            return (
                    await _httpClient.GetFromJsonAsync<List<RepositoryStargazerDto>>(
                        $"/repos/ShaoHans/Abp.RadzenUI/stargazers?per_page={pageSize}&page={pageNumber}&sort=starred&direction=desc"
                    )
                ) ?? [];
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "get repos stargazers occur exception");
            return [];
        }
    }
}
