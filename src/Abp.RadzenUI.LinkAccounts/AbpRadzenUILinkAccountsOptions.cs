namespace Abp.RadzenUI.LinkAccounts;

public class AbpRadzenUILinkAccountsOptions
{
    public string ManagementRoute { get; set; } = LinkedAccountConsts.DefaultManagementRoute;

    public string LoginRoute { get; set; } = LinkedAccountConsts.DefaultLoginRoute;

    public string LinkLoginStartRoute { get; set; } = LinkedAccountConsts.DefaultLinkLoginStartRoute;

    public string SwitchRoute { get; set; } = LinkedAccountConsts.DefaultSwitchRoute;

    public string CallbackRoute { get; set; } = LinkedAccountConsts.DefaultCallbackRoute;

    public string BackRoute { get; set; } = LinkedAccountConsts.DefaultBackRoute;

    public int FlowTokenLifetimeSeconds { get; set; } = 300;

    public int SessionLifetimeHours { get; set; } = 8;

    public int MaxSwitchDepth { get; set; } = 10;

    public bool EnableIndirectLinks { get; set; } = true;

    public void ConfigureDefaults()
    {
        ManagementRoute = string.IsNullOrWhiteSpace(ManagementRoute)
            ? LinkedAccountConsts.DefaultManagementRoute
            : ManagementRoute;
        LoginRoute = string.IsNullOrWhiteSpace(LoginRoute)
            ? LinkedAccountConsts.DefaultLoginRoute
            : LoginRoute;
        LinkLoginStartRoute = string.IsNullOrWhiteSpace(LinkLoginStartRoute)
            ? LinkedAccountConsts.DefaultLinkLoginStartRoute
            : LinkLoginStartRoute;
        SwitchRoute = string.IsNullOrWhiteSpace(SwitchRoute)
            ? LinkedAccountConsts.DefaultSwitchRoute
            : SwitchRoute;
        CallbackRoute = string.IsNullOrWhiteSpace(CallbackRoute)
            ? LinkedAccountConsts.DefaultCallbackRoute
            : CallbackRoute;
        BackRoute = string.IsNullOrWhiteSpace(BackRoute)
            ? LinkedAccountConsts.DefaultBackRoute
            : BackRoute;
        FlowTokenLifetimeSeconds = FlowTokenLifetimeSeconds <= 0 ? 300 : FlowTokenLifetimeSeconds;
        SessionLifetimeHours = SessionLifetimeHours <= 0 ? 8 : SessionLifetimeHours;
        MaxSwitchDepth = MaxSwitchDepth <= 0 ? 10 : MaxSwitchDepth;
    }
}