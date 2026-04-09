using System.Linq.Dynamic.Core;
using Abp.RadzenUI.Application.Contracts.AuditLogs;
using Abp.RadzenUI.Application.Contracts.CommonDtos;
using Abp.RadzenUI.Permissions;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Application.Services;
using Volo.Abp.AuditLogging;
using Volo.Abp.Domain.Entities;
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

    public async Task<AuditLogDetailDto> GetDetailAsync(Guid id)
    {
        await CheckGetPolicyAsync();

        var queryable = await Repository.GetQueryableAsync();
        var auditLog = await AsyncExecuter.FirstOrDefaultAsync(
            queryable
                .Include(x => x.Actions)
                .Include(x => x.EntityChanges)
                .ThenInclude(x => x.PropertyChanges)
                .Where(x => x.Id == id)
        ) ?? throw new EntityNotFoundException(typeof(AuditLog), id);

        var dto = ObjectMapper.Map<AuditLog, AuditLogDetailDto>(auditLog);
        dto.Id = auditLog.Id;
        return dto;
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
