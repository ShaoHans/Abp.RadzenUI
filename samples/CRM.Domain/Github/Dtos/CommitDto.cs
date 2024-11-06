using System;
using System.Text.Json.Serialization;

namespace CRM.Github.Dtos;

public class CommitDto
{
    [JsonPropertyName("commit")]
    public CommitDetailDto Commit { get; set; } = default!;

    [JsonPropertyName("committer")]
    public OutCommitterDto Committer { get; set; } = default!;

    [JsonPropertyName("html_url")]
    public string HtmlUrl { get; set; } = string.Empty;
}

public class CommitDetailDto
{
    [JsonPropertyName("committer")]
    public InnerCommitterDto Committer { get; set; } = default!;

    [JsonPropertyName("message")]
    public string Message { get; set; } = default!;
}

public class InnerCommitterDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = default!;

    [JsonPropertyName("date")]
    public DateTimeOffset Date { get; set; }
}

public class OutCommitterDto
{
    [JsonPropertyName("avatar_url")]
    public string AvatarUrl { get; set; } = string.Empty;
}
