namespace Abp.RadzenUI.Avatar;

public class UploadAvatarRequest
{
    public required Stream Stream { get; init; }

    public required string FileName { get; init; }

    public required string ContentType { get; init; }

    public long ContentLength { get; init; }
}