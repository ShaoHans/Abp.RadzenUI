using Abp.RadzenUI.Localization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Localization;
using Volo.Abp;

namespace Abp.RadzenUI.Avatar;

public class DefaultUploadService(
    IWebHostEnvironment webHostEnvironment,
    IStringLocalizer<AbpRadzenUIResource> localizer
) : IUploadService
{
    public async Task<UploadAvatarResult> UploadAvatarAsync(
        UploadAvatarRequest request,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.ContentLength <= 0 || request.ContentLength > AvatarConsts.MaxFileSize)
        {
            throw new UserFriendlyException(localizer["AvatarFileTooLarge"]);
        }

        if (!AvatarConsts.AllowedContentTypes.Contains(request.ContentType))
        {
            throw new UserFriendlyException(localizer["AvatarInvalidFileType"]);
        }

        var requestExtension = Path.GetExtension(request.FileName);
        if (requestExtension.IsNullOrWhiteSpace() || !AvatarConsts.AllowedExtensions.Contains(requestExtension.ToLowerInvariant()))
        {
            throw new UserFriendlyException(localizer["AvatarInvalidFileType"]);
        }

        Directory.CreateDirectory(GetAvatarPhysicalDirectory());

        await using var inputStream = new MemoryStream();
        await request.Stream.CopyToAsync(inputStream, cancellationToken);

        if (inputStream.Length <= 0 || inputStream.Length > AvatarConsts.MaxFileSize)
        {
            throw new UserFriendlyException(localizer["AvatarFileTooLarge"]);
        }

        inputStream.Position = 0;

        var extension = NormalizeExtension(request.ContentType, requestExtension);
        var fileName = $"{Guid.NewGuid():N}{extension}";
        var relativePath = $"{AvatarConsts.DirectoryName}/{fileName}";
        var physicalPath = Path.Combine(GetAvatarPhysicalDirectory(), fileName);

        await using var outputStream = File.Create(physicalPath);
        await inputStream.CopyToAsync(outputStream, cancellationToken);

        return new UploadAvatarResult
        {
            Url = $"/{relativePath}"
        };
    }

    public Task DeleteAsync(string? avatarUrl, CancellationToken cancellationToken = default)
    {
        if (avatarUrl.IsNullOrWhiteSpace())
        {
            return Task.CompletedTask;
        }

        var relativePath = NormalizeRelativeAvatarPath(avatarUrl);
        if (relativePath is null)
        {
            return Task.CompletedTask;
        }

        var physicalPath = Path.Combine(GetWebRootPath(), relativePath);
        if (File.Exists(physicalPath))
        {
            File.Delete(physicalPath);
        }

        return Task.CompletedTask;
    }

    protected virtual string GetAvatarPhysicalDirectory()
    {
        return Path.Combine(GetWebRootPath(), AvatarConsts.DirectoryName);
    }

    protected virtual string GetWebRootPath()
    {
        return webHostEnvironment.WebRootPath.IsNullOrWhiteSpace()
            ? Path.Combine(webHostEnvironment.ContentRootPath, "wwwroot")
            : webHostEnvironment.WebRootPath;
    }

    protected virtual string NormalizeExtension(string contentType, string requestExtension)
    {
        var normalizedRequestExtension = requestExtension.ToLowerInvariant();

        return contentType.ToLowerInvariant() switch
        {
            "image/jpeg" when normalizedRequestExtension is ".jpg" or ".jpeg" => normalizedRequestExtension,
            "image/png" when normalizedRequestExtension == ".png" => normalizedRequestExtension,
            "image/webp" when normalizedRequestExtension == ".webp" => normalizedRequestExtension,
            "image/gif" when normalizedRequestExtension == ".gif" => normalizedRequestExtension,
            _ => throw new UserFriendlyException(localizer["AvatarInvalidFileType"])
        };
    }

    protected virtual string? NormalizeRelativeAvatarPath(string avatarUrl)
    {
        var relativePath = avatarUrl.Trim();
        if (relativePath.StartsWith('/'))
        {
            relativePath = relativePath[1..];
        }

        relativePath = relativePath.Replace('/', Path.DirectorySeparatorChar);

        var avatarRoot = AvatarConsts.DirectoryName + Path.DirectorySeparatorChar;
        if (!relativePath.StartsWith(avatarRoot, StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        var fullPath = Path.GetFullPath(Path.Combine(GetWebRootPath(), relativePath));
        var avatarDirectory = Path.GetFullPath(GetAvatarPhysicalDirectory()) + Path.DirectorySeparatorChar;
        if (!fullPath.StartsWith(avatarDirectory, StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        return relativePath;
    }
}