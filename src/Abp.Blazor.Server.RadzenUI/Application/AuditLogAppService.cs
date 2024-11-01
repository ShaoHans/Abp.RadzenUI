using System.Linq.Dynamic.Core;
using Abp.RadzenUI.Application.Contracts.AuditLogs;
using Abp.RadzenUI.Application.Contracts.CommonDtos;
using Abp.RadzenUI.Permissions;
using Volo.Abp.Application.Services;
using Volo.Abp.AuditLogging;
using Volo.Abp.Domain.Repositories;

namespace Abp.RadzenUI.Application;

public class AuditLogAppService
    : CrudAppService<
        AuditLog,
        AuditLogDto,
        Guid,
        GetAuditLogsInput,
        EmptyCreateDto,
        EmptyUpdateDto
    >,
        IAuditLogAppService
{
    public AuditLogAppService(IRepository<AuditLog, Guid> repository)
        : base(repository)
    {
        GetPolicyName = RadzenUIPermissions.AuditLogs.Default;
        GetListPolicyName = RadzenUIPermissions.AuditLogs.Default;
    }

    protected override async Task<IQueryable<AuditLog>> CreateFilteredQueryAsync(
        GetAuditLogsInput input
    )
    {
        if (input.Sorting.IsNullOrEmpty())
        {
            input.Sorting = $"{nameof(AuditLog.ExecutionTime)} desc";
        }

        var query = await base.CreateFilteredQueryAsync(input);

        if (!string.IsNullOrEmpty(input.Filter))
        {
            query = query.Where(input.Filter);
        }

        return query;
    }
}
