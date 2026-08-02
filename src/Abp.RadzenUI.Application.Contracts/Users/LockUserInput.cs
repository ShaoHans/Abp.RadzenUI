namespace Abp.RadzenUI.Application.Contracts.Users;

public class LockUserInput
{
    /// <summary>
    /// UTC time until which the user remains locked out. Must be in the future.
    /// A far-future value (e.g. year 9999) is treated as "locked indefinitely".
    /// </summary>
    public DateTimeOffset LockoutEnd { get; set; }
}
