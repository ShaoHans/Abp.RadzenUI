namespace Abp.RadzenUI.LinkAccounts;

public static class LinkedAccountConsts
{
    public const string FeatureName = "AbpRadzenUI.LinkedAccounts";

    public const string DefaultManagementRoute = "/account/linked-accounts";

    public const string DefaultLoginRoute = "/account/login";

    public const string DefaultLinkLoginStartRoute = "/account/linked-accounts/link-login";

    public const string DefaultSwitchRoute = "/account/linked-accounts/switch";

    public const string DefaultCallbackRoute = "/account/linked-accounts/callback";

    public const string DefaultBackRoute = "/account/linked-accounts/back";

    public const string LinkFlowCacheKeyPrefix = "AbpRadzenUI:LinkedAccounts:Flow:";

    public const string SessionCacheKeyPrefix = "AbpRadzenUI:LinkedAccounts:Session:";
}