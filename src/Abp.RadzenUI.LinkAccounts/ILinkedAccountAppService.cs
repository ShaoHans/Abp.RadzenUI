using Abp.RadzenUI.LinkAccounts.Dtos;

namespace Abp.RadzenUI.LinkAccounts;

public interface ILinkedAccountAppService
{
    Task<IReadOnlyList<LinkedAccountDto>> GetListAsync(CancellationToken cancellationToken = default);

    Task<LinkedAccountSessionDto?> GetCurrentSessionAsync(CancellationToken cancellationToken = default);

    Task<LinkedAccountFlowDto> CreateLinkLoginAsync(CreateLinkedAccountLinkDto input, CancellationToken cancellationToken = default);

    Task<LinkedAccountFlowDto> CreateSwitchAsync(LinkedAccountSwitchDto input, CancellationToken cancellationToken = default);

    Task DeleteAsync(LinkedAccountSwitchDto input, CancellationToken cancellationToken = default);
}