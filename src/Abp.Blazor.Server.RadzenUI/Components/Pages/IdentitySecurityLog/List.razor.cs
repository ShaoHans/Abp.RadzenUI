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
    }

    protected override Task UpdateGetListInputAsync(LoadDataArgs args)
    {
        GetListInput.LoginTimeStart = _loginTimeStart;
        GetListInput.LoginTimeEnd = _loginTimeEnd;
        GetListInput.UserName = _userName;
        _defaultPageSize = 20;

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

    private async Task ResetAsync()
    {
        _loginTimeStart = null;
        _loginTimeEnd = null;
        _userName = null;
        _defaultPageSize = 20;
        await SearchAsync();
    }
}