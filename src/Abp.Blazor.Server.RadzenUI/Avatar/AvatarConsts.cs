using System.Collections.ObjectModel;

namespace Abp.RadzenUI.Avatar;

public static class AvatarConsts
{
    public const string ExtraPropertyName = "AvatarUrl";
    public const string DirectoryName = "avatars";
    public const long MaxFileSize = 4 * 1024 * 1024;
    public const int OutputSize = 256;

    public static readonly IReadOnlyCollection<string> AllowedContentTypes =
        new ReadOnlyCollection<string>(
            ["image/jpeg", "image/png", "image/webp", "image/gif"]
        );

    public static readonly IReadOnlyCollection<string> AllowedExtensions =
        new ReadOnlyCollection<string>([".jpg", ".jpeg", ".png", ".webp", ".gif"]);
}