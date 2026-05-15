using Abp.RadzenUI.Application.Contracts.CommonDtos;
using Abp.RadzenUI.Application.Contracts.IdentitySecurityLogs;
using Abp.RadzenUI.Permissions;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;

namespace Abp.RadzenUI.Application;

public class IdentitySecurityLogAppService
    : CrudAppService<
        IdentitySecurityLog,
        IdentitySecurityLogDto,
        Guid,
        GetIdentitySecurityLogsInput,
        EmptyCreateDto,
        EmptyUpdateDto
    >,
        IIdentitySecurityLogAppService
{
    public IdentitySecurityLogAppService(IRepository<IdentitySecurityLog, Guid> repository)
        : base(repository)
    {
        GetPolicyName = RadzenUIPermissions.SecurityLogs.Default;
        GetListPolicyName = RadzenUIPermissions.SecurityLogs.Default;
    }

    protected override async Task<IQueryable<IdentitySecurityLog>> CreateFilteredQueryAsync(
        GetIdentitySecurityLogsInput input
    )
    {
        if (input.Sorting.IsNullOrEmpty())
        {
            input.Sorting = $"{nameof(IdentitySecurityLog.CreationTime)} desc";
        }

        var query = await base.CreateFilteredQueryAsync(input);

        if (!string.IsNullOrWhiteSpace(input.UserName))
        {
            query = query.Where(x => x.UserName != null && x.UserName.Contains(input.UserName));
        }

        if (input.LoginTimeStart.HasValue)
        {
            query = query.Where(x => x.CreationTime >= input.LoginTimeStart.Value);
        }

        if (input.LoginTimeEnd.HasValue)
        {
            query = query.Where(x => x.CreationTime <= input.LoginTimeEnd.Value);
        }

        return query;
    }
}