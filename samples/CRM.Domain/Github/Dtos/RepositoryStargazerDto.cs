using System;
using System.Text.Json.Serialization;

namespace CRM.Github.Dtos;

public class RepositoryStargazerDto
{
    [JsonPropertyName("starred_at")]
    public DateTime StarredAt { get; set; }

    [JsonPropertyName("user")]
    public RepositoryStargazerUserDto User { get; set; } = default!;
}

public class RepositoryStargazerUserDto
{
    [JsonPropertyName("login")]
    public string UserName { get; set; } = default!;

    [JsonPropertyName("avatar_url")]
    public string AvatarUrl { get; set; } = string.Empty;

    [JsonPropertyName("url")]
    public string HomePageUrl { get; set; } = string.Empty;
}
