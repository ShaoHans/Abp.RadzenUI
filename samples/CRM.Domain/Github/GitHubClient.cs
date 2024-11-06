using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CRM.Github.Dtos;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
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
            logger.LogInformation(
                "GitHub Token is {0}",
                _httpClient.DefaultRequestHeaders.Authorization?.ToString()
            );
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
            logger.LogInformation(
                "GitHub Token is {0}",
                _httpClient.DefaultRequestHeaders.Authorization?.ToString()
            );
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation(
                "Accept",
                "application/vnd.github.star+json"
            );
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

    public async Task<List<CommitDto>> GetRepositoryCommitsAsync(
        int pageSize = 20,
        int pageNumber = 1
    )
    {
        try
        {
            return (
                    await _httpClient.GetFromJsonAsync<List<CommitDto>>(
                        $"/repos/ShaoHans/Abp.RadzenUI/commits?per_page={pageSize}&page={pageNumber}"
                    )
                ) ?? [];
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "get repos commits occur exception");
            return [];
        }
    }

    public async Task<int> GetRepositoryCommitCountAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync(
                "/repos/ShaoHans/Abp.RadzenUI/commits?per_page=1&page=1"
            );
            var link = response.Headers.GetValues("Link").First();
            var matches = Regex.Matches(link, @"page=(\d+)");
            if (matches.Count > 0)
            {
                return int.Parse(matches[^1].Groups[1].Value);
            }
            return 0;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "get repos commits occur exception");
            return 0;
        }
    }
}
