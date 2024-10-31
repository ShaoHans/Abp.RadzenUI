using Abp.RadzenUI.Application.Contracts.CommonDtos;
using Volo.Abp.Application.Services;

namespace Abp.RadzenUI.Application.Contracts.AuditLogs;

public interface IAuditLogAppService
    : ICrudAppService<AuditLogDto, Guid, GetAuditLogsInput, EmptyCreateDto, EmptyUpdateDto>
{ }
