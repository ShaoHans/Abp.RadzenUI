using System.ComponentModel.DataAnnotations;
using Abp.RadzenUI.Messages;
using Volo.Abp.Application.Dtos;

namespace Abp.RadzenUI.Application.Contracts.Messages;

public class SaveUserMessageDto : ExtensibleEntityDto
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    public string Title { get; set; } = default!;

    [Required]
    public string Content { get; set; } = default!;

    [Required]
    [StringLength(MessageConsts.MaxMessageTypeLength)]
    public string MessageType { get; set; } = default!;
}