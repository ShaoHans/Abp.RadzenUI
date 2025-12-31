using System.ComponentModel.DataAnnotations;
using Volo.Abp.Identity;
using Volo.Abp.ObjectExtending;
using Volo.Abp.Validation;

namespace Abp.RadzenUI.Application.Contracts.Organizations;

public class OrganizationUnitCreateDto : ExtensibleObject
{
    [Required]
    [DynamicStringLength(typeof(OrganizationUnitConsts), nameof(OrganizationUnitConsts.MaxDisplayNameLength))]
    public required string DisplayName { get; set; }

    public Guid? ParentId { get; set; }
}

