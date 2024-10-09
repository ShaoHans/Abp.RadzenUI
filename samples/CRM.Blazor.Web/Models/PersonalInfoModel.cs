using Volo.Abp.ObjectExtending;

namespace CRM.Blazor.Web.Models;

public class PersonalInfoModel : ExtensibleObject
{
    public string UserName { get; set; } = default!;

    public string Email { get; set; } = default!;

    public string? Name { get; set; }

    public string? Surname { get; set; }

    public string? PhoneNumber { get; set; }

    public bool PhoneNumberConfirmed { get; set; }

    public bool EmailConfirmed { get; set; }

    public string ConcurrencyStamp { get; set; } = default!;
}
