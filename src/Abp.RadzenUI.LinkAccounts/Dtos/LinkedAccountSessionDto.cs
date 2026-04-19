namespace Abp.RadzenUI.LinkAccounts.Dtos;

public class LinkedAccountSessionDto
{
    public string? SessionId { get; set; }

    public Guid? OriginTenantId { get; set; }

    public string? OriginTenantName { get; set; }

    public Guid? OriginUserId { get; set; }

    public string? OriginUserName { get; set; }

    public Guid? CurrentTenantId { get; set; }

    public string? CurrentTenantName { get; set; }

    public Guid? CurrentUserId { get; set; }

    public string? CurrentUserName { get; set; }

    public bool CanGoBack { get; set; }
}