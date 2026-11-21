using Abp.RadzenUI.Application.Contracts.CommonDtos;
using Abp.RadzenUI.Application.Contracts.IdentitySecurityLogs;
using Abp.RadzenUI.Localization;
using Radzen;

namespace Abp.RadzenUI.Components.Pages.IdentitySecurityLog;

public partial class List
{
    private DateTime? _loginTimeStart;
    private DateTime? _loginTimeEnd;
    private string? _userName;

    public List()
    {
        ObjectMapperContext = typeof(AbpRadzenUIModule);
        LocalizationResource = typeof(AbpRadzenUIResource);
        _defaultPageSize = 20;
    }

    protected override Task UpdateGetListInputAsync(LoadDataArgs args)
    {
        GetListInput.LoginTimeStart = _loginTimeStart;
        GetListInput.LoginTimeEnd = _loginTimeEnd;
        GetListInput.UserName = _userName;

        return base.UpdateGetListInputAsync(args);
    }

    protected override Task<EmptyUpdateDto> SetEditDialogModelAsync(IdentitySecurityLogDto dto)
    {
        return Task.FromResult(new EmptyUpdateDto());
    }

    private async Task SearchAsync()
    {
        if (_grid is not null)
        {
            await _grid.Reload();
        }
    }

    /// <summary>
    /// Colours an ABP security-log action (IdentitySecurityLogActionConsts):
    /// successful logins green, rejected attempts red, everything else
    /// (logout, password change, ...) neutral info.
    /// </summary>
    private static BadgeStyle GetActionBadgeStyle(string? action)
    {
        if (string.IsNullOrEmpty(action))
        {
            return BadgeStyle.Base;
        }

        if (action.Contains("Succeeded", StringComparison.OrdinalIgnoreCase))
        {
            return BadgeStyle.Success;
        }

        return action.Contains("Failed", StringComparison.OrdinalIgnoreCase)
               || action.Contains("Invalid", StringComparison.OrdinalIgnoreCase)
               || action.Contains("Locked", StringComparison.OrdinalIgnoreCase)
               || action.Contains("NotAllowed", StringComparison.OrdinalIgnoreCase)
            ? BadgeStyle.Danger
            : BadgeStyle.Info;
    }

    private async Task ResetAsync()
    {
        _loginTimeStart = null;
        _loginTimeEnd = null;
        _userName = null;
        await SearchAsync();
    }
}