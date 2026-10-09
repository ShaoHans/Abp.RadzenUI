namespace Abp.RadzenUI.ObjectExtending;

/// <summary>
/// How an extension property behaves on the user's own profile page
/// (<c>/account/manage</c>, "Personal Info" tab). It does not affect the
/// administrator's user create/edit dialogs, which keep using ABP's
/// <c>UI.CreateModal</c> / <c>UI.EditModal</c> settings.
/// </summary>
public enum ProfileExtensionPropertyMode
{
    /// <summary>Not rendered on the profile page and never accepted from it.</summary>
    Hidden,

    /// <summary>Rendered as a read-only field; changes sent from the profile page are ignored.</summary>
    ReadOnly,

    /// <summary>Rendered as an editable field and persisted on save.</summary>
    Editable,
}
