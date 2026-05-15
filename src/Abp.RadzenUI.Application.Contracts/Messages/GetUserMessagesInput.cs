using Abp.RadzenUI.Application.Contracts.CommonDtos;

namespace Abp.RadzenUI.Application.Contracts.Messages;

public class GetUserMessagesInput : PagedFilterAndSortedResultRequestDto
{
    public string? MessageType { get; set; }

    public MessageReadStatus? ReadStatus { get; set; }

    public DateTime? ReceivedTimeStart { get; set; }

    public DateTime? ReceivedTimeEnd { get; set; }
}