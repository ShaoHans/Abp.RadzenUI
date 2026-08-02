using Abp.RadzenUI.Application.Contracts.Messages;
using Abp.RadzenUI.Messages;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace Abp.RadzenUI.Application.Messages;

public class MessageTypeLookupAppService : ApplicationService, IMessageTypeLookupAppService
{
    protected IRepository<UserMessage, Guid> Repository { get; }

    public MessageTypeLookupAppService(IRepository<UserMessage, Guid> repository)
    {
        Repository = repository;
    }

    public virtual async Task<ListResultDto<MessageTypeLookupDto>> GetListAsync(
        GetMessageTypeLookupInput input
    )
    {
        var currentUserId = CurrentUser.GetId();
        var query = (await Repository.GetQueryableAsync())
            .Where(x => x.UserId == currentUserId)
            .Select(x => x.MessageType)
            .Distinct();

        if (!string.IsNullOrWhiteSpace(input.Filter))
        {
            query = query.Where(x => x.Contains(input.Filter));
        }

        var items = await AsyncExecuter.ToListAsync(query.OrderBy(x => x));

        return new ListResultDto<MessageTypeLookupDto>(
            items.Select(x => new MessageTypeLookupDto { Value = x, Text = x }).ToList()
        );
    }
}