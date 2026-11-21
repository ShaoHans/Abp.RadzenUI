namespace Abp.RadzenUI.Components.Pages.User;

/// <summary>
/// A role shown in the user dialog's role picker.
/// </summary>
/// <param name="Name">Role name.</param>
/// <param name="InheritedFromOrganizationUnit">
/// The user gets this role through an organization unit, so it cannot be toggled here.
/// </param>
public record UserRoleOption(string Name, bool InheritedFromOrganizationUnit = false);
