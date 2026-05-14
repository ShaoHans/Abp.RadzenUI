using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Abp.RadzenUI.Application.Contracts.Messages;

public interface IMessageTypeLookupAppService : IApplicationService
{
    Task<ListResultDto<MessageTypeLookupDto>> GetListAsync(GetMessageTypeLookupInput input);
}