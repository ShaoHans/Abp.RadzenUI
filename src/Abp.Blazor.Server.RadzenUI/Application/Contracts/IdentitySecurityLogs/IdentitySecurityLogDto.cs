using Volo.Abp.Application.Dtos;

namespace Abp.RadzenUI.Application.Contracts.IdentitySecurityLogs;

public class IdentitySecurityLogDto : EntityDto<Guid>
{
    public string? ApplicationName { get; set; }

    public string? UserName { get; set; }

    public string? TenantName { get; set; }

    public string? Action { get; set; }

    public string? ClientIpAddress { get; set; }

    public DateTime CreationTime { get; set; }
}