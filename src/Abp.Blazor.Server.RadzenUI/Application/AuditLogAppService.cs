using Abp.RadzenUI.Application.Contracts.AuditLogs;
using Abp.RadzenUI.Application.Contracts.CommonDtos;
using Volo.Abp.Application.Services;
using Volo.Abp.AuditLogging;
using Volo.Abp.Domain.Repositories;

namespace Abp.RadzenUI.Application;

public class AuditLogAppService(IRepository<AuditLog, Guid> repository)
    : CrudAppService<
        AuditLog,
        AuditLogDto,
        Guid,
        GetAuditLogsInput,
        EmptyCreateDto,
        EmptyUpdateDto
    >(repository),
        IAuditLogAppService { }
