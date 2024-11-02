using System.Text.Json.Serialization;

namespace CRM.Github.Dtos;

public class RepositoryDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = default!;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("stargazers_count")]
    public int StargazersCount { get; set; }
}
