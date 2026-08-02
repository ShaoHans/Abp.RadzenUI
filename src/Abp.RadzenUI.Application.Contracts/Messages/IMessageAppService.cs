using Volo.Abp.Application.Services;

namespace Abp.RadzenUI.Application.Contracts.Messages;

public interface IMessageAppService
    : ICrudAppService<UserMessageDto, Guid, GetUserMessagesInput, SaveUserMessageDto, SaveUserMessageDto>
{
    Task<long> GetUnreadCountAsync();

    Task MarkAsReadAsync(Guid id);

    Task BatchMarkAsReadAsync(MarkUserMessagesAsReadInput input);

    Task MarkAllAsReadAsync();
}