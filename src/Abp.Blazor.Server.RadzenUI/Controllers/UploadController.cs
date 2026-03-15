using Abp.RadzenUI.Avatar;
using Abp.RadzenUI.Localization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Volo.Abp;
using Volo.Abp.Authorization;
using Volo.Abp.Data;
using Volo.Abp.Identity;

namespace Abp.RadzenUI.Controllers;

[Authorize]
[Route("api/account/avatar")]
public class UploadController(
    IIdentityUserRepository identityUserRepository,
    IUploadService uploadService,
    IStringLocalizer<AbpRadzenUIResource> localizer
) : AbpRadzenControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetCurrentAvatarAsync(CancellationToken cancellationToken)
    {
        var user = await GetCurrentUserAsync(cancellationToken);

        return Ok(
            new
            {
                avatarUrl = user.GetProperty<string>(AvatarConsts.ExtraPropertyName),
                hasAvatar = !user.GetProperty<string>(AvatarConsts.ExtraPropertyName).IsNullOrWhiteSpace()
            }
        );
    }

    [HttpPost]
    [RequestSizeLimit(AvatarConsts.MaxFileSize)]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> UploadAsync(IFormFile file, CancellationToken cancellationToken)
    {
        if (file is null)
        {
            return BadRequest(new { error = localizer["AvatarNoFileSelected"].Value });
        }

        if (file.Length <= 0 || file.Length > AvatarConsts.MaxFileSize)
        {
            return BadRequest(new { error = localizer["AvatarFileTooLarge"].Value });
        }

        if (!AvatarConsts.AllowedContentTypes.Contains(file.ContentType))
        {
            return BadRequest(new { error = localizer["AvatarInvalidFileType"].Value });
        }

        var user = await GetCurrentUserAsync(cancellationToken);
        var previousAvatarUrl = user.GetProperty<string>(AvatarConsts.ExtraPropertyName);

        UploadAvatarResult result;
        try
        {
            await using var stream = file.OpenReadStream();
            result = await uploadService.UploadAvatarAsync(
                new UploadAvatarRequest
                {
                    Stream = stream,
                    FileName = file.FileName,
                    ContentType = file.ContentType,
                    ContentLength = file.Length
                },
                cancellationToken
            );
        }
        catch (UserFriendlyException ex)
        {
            return BadRequest(new { error = ex.Message });
        }

        try
        {
            user.SetProperty(AvatarConsts.ExtraPropertyName, result.Url, validate: false);

            await identityUserRepository.UpdateAsync(user, autoSave: true, cancellationToken: cancellationToken);
        }
        catch
        {
            await uploadService.DeleteAsync(result.Url, cancellationToken);
            throw;
        }

        if (!previousAvatarUrl.IsNullOrWhiteSpace() && previousAvatarUrl != result.Url)
        {
            await uploadService.DeleteAsync(previousAvatarUrl, cancellationToken);
        }

        return Ok(new { avatarUrl = result.Url });
    }

    [HttpDelete]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> DeleteAsync(CancellationToken cancellationToken)
    {
        var user = await GetCurrentUserAsync(cancellationToken);
        var avatarUrl = user.GetProperty<string>(AvatarConsts.ExtraPropertyName);

        user.RemoveProperty(AvatarConsts.ExtraPropertyName);

        await identityUserRepository.UpdateAsync(user, autoSave: true, cancellationToken: cancellationToken);
        await uploadService.DeleteAsync(avatarUrl, cancellationToken);

        return NoContent();
    }

    protected virtual async Task<IdentityUser> GetCurrentUserAsync(CancellationToken cancellationToken)
    {
        if (!CurrentUser.Id.HasValue)
        {
            throw new AbpAuthorizationException();
        }

        return await identityUserRepository.GetAsync(CurrentUser.Id.Value, cancellationToken: cancellationToken);
    }
}