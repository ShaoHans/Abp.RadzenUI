using Volo.Abp.Application.Dtos;

namespace Abp.RadzenUI.Application.Contracts.Messages;

public class UserMessageDto : FullAuditedEntityDto<Guid>
{
    public Guid UserId { get; set; }

    public string Title { get; set; } = default!;

    public string Content { get; set; } = default!;

    public string MessageType { get; set; } = default!;

    public bool IsRead { get; set; }

    public DateTime? ReadTime { get; set; }
}