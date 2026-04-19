namespace Abp.RadzenUI.LinkAccounts.Dtos;

public sealed class LinkedAccountSessionDisplayModel
{
    public static LinkedAccountSessionDisplayModel Create(LinkedAccountSessionDto? session, string hostDisplay)
    {
        if (session == null)
        {
            return new LinkedAccountSessionDisplayModel
            {
                OriginTenantDisplay = hostDisplay,
                CurrentTenantDisplay = hostDisplay,
            };
        }

        var originTenantDisplay = session.OriginTenantName ?? hostDisplay;
        var currentTenantDisplay = session.CurrentTenantName ?? hostDisplay;
        var originUserDisplay = session.OriginUserName ?? string.Empty;
        var currentUserDisplay = session.CurrentUserName ?? string.Empty;

        return new LinkedAccountSessionDisplayModel
        {
            HasSession = true,
            CanGoBack = session.CanGoBack,
            OriginTenantDisplay = originTenantDisplay,
            OriginUserDisplay = originUserDisplay,
            CurrentTenantDisplay = currentTenantDisplay,
            CurrentUserDisplay = currentUserDisplay,
            OriginAccountDisplay = $"{originTenantDisplay} / {originUserDisplay}",
            CurrentAccountDisplay = $"{currentTenantDisplay} / {currentUserDisplay}",
        };
    }

    public bool HasSession { get; private init; }

    public bool CanGoBack { get; private init; }

    public string OriginTenantDisplay { get; private init; } = string.Empty;

    public string OriginUserDisplay { get; private init; } = string.Empty;

    public string CurrentTenantDisplay { get; private init; } = string.Empty;

    public string CurrentUserDisplay { get; private init; } = string.Empty;

    public string OriginAccountDisplay { get; private init; } = string.Empty;

    public string CurrentAccountDisplay { get; private init; } = string.Empty;
}