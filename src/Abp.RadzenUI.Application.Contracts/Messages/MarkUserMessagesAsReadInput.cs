using Volo.Abp.ObjectExtending;

namespace Abp.RadzenUI.Application.Contracts.Messages;

public class MarkUserMessagesAsReadInput : ExtensibleObject
{
    public List<Guid> Ids { get; set; } = [];
}