using Volo.Abp.ObjectExtending;

namespace Abp.RadzenUI.Application.Contracts.Messages;

public class GetMessageTypeLookupInput : ExtensibleObject
{
    public string? Filter { get; set; }
}