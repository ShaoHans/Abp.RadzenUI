using Abp.RadzenUI.Application.Contracts.CommonDtos;
using Volo.Abp.Application.Services;

namespace Abp.RadzenUI.Application.Contracts.IdentitySecurityLogs;

public interface IIdentitySecurityLogAppService
    : ICrudAppService<
        IdentitySecurityLogDto,
        Guid,
        GetIdentitySecurityLogsInput,
        EmptyCreateDto,
        EmptyUpdateDto
    > { }