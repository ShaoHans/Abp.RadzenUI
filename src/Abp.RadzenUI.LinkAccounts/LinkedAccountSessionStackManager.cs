namespace Abp.RadzenUI.LinkAccounts;

public static class LinkedAccountSessionStackManager
{
    public static void ApplySwitch(
        LinkedAccountSessionCacheItem session,
        LinkedAccountUserInfo target,
        int maxSwitchDepth)
    {
        var existingIndex = session.Stack.FindIndex(x => x.UserId == target.UserId && x.TenantId == target.TenantId);
        if (existingIndex >= 0)
        {
            session.Stack.RemoveRange(existingIndex + 1, session.Stack.Count - existingIndex - 1);
            return;
        }

        if (session.Stack.Count >= maxSwitchDepth)
        {
            throw new InvalidOperationException("Linked account switch depth exceeded.");
        }

        session.Stack.Add(target);
    }
}