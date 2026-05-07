using Abp.RadzenUI.Application.Contracts.AuditLogs;
using Abp.RadzenUI.Application.Contracts.CommonDtos;
using Microsoft.AspNetCore.Components;
using Radzen;
using Volo.Abp.AuditLogging.Localization;

namespace Abp.RadzenUI.Components.Pages.AuditLog;

public partial class List : IDisposable
{
    [Inject]
    protected SideDialogState<AuditLogDto> AuditLogDialogState { get; set; } = default!;

    private SideDialogCoordinator<AuditLogDto> _sideDialogCoordinator = default!;

    public List()
    {
        ObjectMapperContext = typeof(AbpRadzenUIModule);
        LocalizationResource = typeof(AuditLoggingResource);
        _defaultPageSize = 20;
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        _sideDialogCoordinator = new SideDialogCoordinator<AuditLogDto>(DialogService, AuditLogDialogState);
        _sideDialogCoordinator.Attach();
    }

    protected override async Task UpdateGetListInputAsync(LoadDataArgs args)
    {
        GetListInput.Filter = args.Filter;
        await base.UpdateGetListInputAsync(args);
    }

    protected override Task<EmptyUpdateDto> SetEditDialogModelAsync(AuditLogDto dto)
    {
        return Task.FromResult(new EmptyUpdateDto());
    }

    private async Task OpenDetailDialogAsync(AuditLogDto dto)
    {
        await _sideDialogCoordinator.OpenDetailAsync<AuditLogDto, Detail>(
            dto,
            UL["AuditLog:ViewDetail"],
            "AuditLog",
            "980px"
        );
    }

    public void Dispose()
    {
        _sideDialogCoordinator.Detach();
    }
}
