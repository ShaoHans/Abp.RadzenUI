using Abp.RadzenUI.Application.Contracts.AuditLogs;
using Abp.RadzenUI.Application.Contracts.CommonDtos;
using Radzen;
using Volo.Abp.AuditLogging.Localization;

namespace Abp.RadzenUI.Components.Pages.AuditLog;

public partial class List
{
    public List()
    {
        ObjectMapperContext = typeof(AbpRadzenUIModule);
        LocalizationResource = typeof(AuditLoggingResource);
    }

    protected override async Task UpdateGetListInputAsync(LoadDataArgs args)
    {
        GetListInput.Filter = args.Filter;
        _defaultPageSize = 20;
        await base.UpdateGetListInputAsync(args);
    }

    protected override Task<EmptyUpdateDto> SetEditDialogModelAsync(AuditLogDto dto)
    {
        return Task.FromResult(new EmptyUpdateDto());
    }
}
