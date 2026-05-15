using Volo.Abp.Application.Dtos;

namespace Abp.RadzenUI.Application.Contracts.IdentitySecurityLogs;

public class GetIdentitySecurityLogsInput : PagedAndSortedResultRequestDto
{
    public DateTime? LoginTimeStart { get; set; }

    public DateTime? LoginTimeEnd { get; set; }

    public string? UserName { get; set; }
}