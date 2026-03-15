namespace Abp.RadzenUI.Avatar;

public interface IUploadService
{
    Task<UploadAvatarResult> UploadAvatarAsync(
        UploadAvatarRequest request,
        CancellationToken cancellationToken = default
    );

    Task DeleteAsync(string? avatarUrl, CancellationToken cancellationToken = default);
}