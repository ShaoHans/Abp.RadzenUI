using Abp.RadzenUI.Application.Contracts.Messages;
using Abp.RadzenUI.Localization;
using Abp.RadzenUI.Messages;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace Abp.RadzenUI.Application.Messages;

public class MessageAppService
    : CrudAppService<
        UserMessage,
        UserMessageDto,
        Guid,
        GetUserMessagesInput,
        SaveUserMessageDto,
        SaveUserMessageDto
    >,
        IMessageAppService
{
    public MessageAppService(IRepository<UserMessage, Guid> repository)
        : base(repository)
    {
        LocalizationResource = typeof(AbpRadzenUIResource);
    }

    public override async Task<UserMessageDto> CreateAsync(SaveUserMessageDto input)
    {
        await CheckCreatePolicyAsync();

        var entity = ObjectMapper.Map<SaveUserMessageDto, UserMessage>(input);
        entity.TenantId = CurrentTenant.Id;
        entity.Title = TruncateTitle(input.Title);
        entity.IsRead = false;
        entity.ReadTime = null;

        await Repository.InsertAsync(entity, autoSave: true);

        return await MapToGetOutputDtoAsync(entity);
    }

    public override async Task<UserMessageDto> UpdateAsync(Guid id, SaveUserMessageDto input)
    {
        await CheckUpdatePolicyAsync();

        var entity = await Repository.GetAsync(id);
        ObjectMapper.Map(input, entity);
        entity.Title = TruncateTitle(input.Title);

        await Repository.UpdateAsync(entity, autoSave: true);

        return await MapToGetOutputDtoAsync(entity);
    }

    public override async Task<UserMessageDto> GetAsync(Guid id)
    {
        await CheckGetPolicyAsync();

        var entity = await GetCurrentUserMessageEntityAsync(id);

        if (!entity.IsRead)
        {
            entity.IsRead = true;
            entity.ReadTime = Clock.Now;
            await Repository.UpdateAsync(entity, autoSave: true);
        }

        return await MapToGetOutputDtoAsync(entity);
    }

    public override async Task DeleteAsync(Guid id)
    {
        await CheckDeletePolicyAsync();

        var entity = await GetCurrentUserMessageEntityAsync(id);
        await Repository.DeleteAsync(entity, autoSave: true);
    }

    public async Task<long> GetUnreadCountAsync()
    {
        var currentUserId = GetCurrentUserId();
        var query = await Repository.GetQueryableAsync();

        return await AsyncExecuter.LongCountAsync(
            query.Where(x => x.UserId == currentUserId && !x.IsRead)
        );
    }

    public async Task MarkAsReadAsync(Guid id)
    {
        var entity = await GetCurrentUserMessageEntityAsync(id);

        if (entity.IsRead)
        {
            return;
        }

        entity.IsRead = true;
        entity.ReadTime = Clock.Now;
        await Repository.UpdateAsync(entity, autoSave: true);
    }

    public async Task BatchMarkAsReadAsync(MarkUserMessagesAsReadInput input)
    {
        if (input.Ids.Count == 0)
        {
            return;
        }

        var currentUserId = GetCurrentUserId();
        var ids = input.Ids.Distinct().ToList();
        var query = await Repository.GetQueryableAsync();
        var entities = await AsyncExecuter.ToListAsync(
            query.Where(x => ids.Contains(x.Id) && x.UserId == currentUserId && !x.IsRead)
        );

        if (entities.Count == 0)
        {
            return;
        }

        foreach (var entity in entities)
        {
            entity.IsRead = true;
            entity.ReadTime = Clock.Now;
        }

        await Repository.UpdateManyAsync(entities, autoSave: true);
    }

    public async Task MarkAllAsReadAsync()
    {
        var currentUserId = GetCurrentUserId();
        var query = await Repository.GetQueryableAsync();
        var entities = await AsyncExecuter.ToListAsync(
            query.Where(x => x.UserId == currentUserId && !x.IsRead)
        );

        if (entities.Count == 0)
        {
            return;
        }

        foreach (var entity in entities)
        {
            entity.IsRead = true;
            entity.ReadTime = Clock.Now;
        }

        await Repository.UpdateManyAsync(entities, autoSave: true);
    }

    protected override async Task<IQueryable<UserMessage>> CreateFilteredQueryAsync(
        GetUserMessagesInput input
    )
    {
        if (string.IsNullOrWhiteSpace(input.Sorting))
        {
            input.Sorting = $"{nameof(UserMessage.CreationTime)} desc";
        }

        var currentUserId = GetCurrentUserId();
        var query = (await Repository.GetQueryableAsync()).Where(x => x.UserId == currentUserId);

        if (!string.IsNullOrWhiteSpace(input.Filter))
        {
            query = query.Where(x => x.Title.Contains(input.Filter));
        }

        if (!string.IsNullOrWhiteSpace(input.MessageType))
        {
            query = query.Where(x => x.MessageType == input.MessageType);
        }

        if (input.ReadStatus == MessageReadStatus.Read)
        {
            query = query.Where(x => x.IsRead);
        }
        else if (input.ReadStatus == MessageReadStatus.Unread)
        {
            query = query.Where(x => !x.IsRead);
        }

        if (input.ReceivedTimeStart.HasValue)
        {
            query = query.Where(x => x.CreationTime >= input.ReceivedTimeStart.Value);
        }

        if (input.ReceivedTimeEnd.HasValue)
        {
            query = query.Where(x => x.CreationTime <= input.ReceivedTimeEnd.Value);
        }

        return query;
    }

    protected override Task<UserMessage> MapToEntityAsync(SaveUserMessageDto createInput)
    {
        createInput.Title = TruncateTitle(createInput.Title);
        return base.MapToEntityAsync(createInput);
    }

    protected override Task MapToEntityAsync(SaveUserMessageDto updateInput, UserMessage entity)
    {
        updateInput.Title = TruncateTitle(updateInput.Title);
        return base.MapToEntityAsync(updateInput, entity);
    }

    private async Task<UserMessage> GetCurrentUserMessageEntityAsync(Guid id)
    {
        var currentUserId = GetCurrentUserId();
        var query = await Repository.GetQueryableAsync();

        return await AsyncExecuter.FirstOrDefaultAsync(
                query.Where(x => x.Id == id && x.UserId == currentUserId)
            )
            ?? throw new EntityNotFoundException(typeof(UserMessage), id);
    }

    private Guid GetCurrentUserId()
    {
        return CurrentUser.GetId();
    }

    private static string TruncateTitle(string title)
    {
        title = title.Trim();

        if (title.Length <= MessageConsts.MaxTitleLength)
        {
            return title;
        }

        return title[..MessageConsts.MaxTitleLength];
    }
}