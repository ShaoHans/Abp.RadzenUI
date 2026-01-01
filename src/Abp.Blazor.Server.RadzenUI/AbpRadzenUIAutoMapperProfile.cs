using Abp.RadzenUI.Application.Contracts.AuditLogs;
using Abp.RadzenUI.Application.Contracts.Organizations;
using Abp.RadzenUI.Models;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Account;
using Volo.Abp.AuditLogging;
using Volo.Abp.Identity;
using Volo.Abp.Mapperly;
using Volo.Abp.SettingManagement;

namespace Abp.RadzenUI;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
[MapExtraProperties]
public partial class ProfileDtoToPersonalInfoModelMapper : MapperBase<ProfileDto, PersonalInfoModel>
{
    [MapperIgnoreTarget(nameof(PersonalInfoModel.PhoneNumberConfirmed))]
    [MapperIgnoreTarget(nameof(PersonalInfoModel.EmailConfirmed))]
    public override partial PersonalInfoModel Map(ProfileDto source);

    [MapperIgnoreTarget(nameof(PersonalInfoModel.PhoneNumberConfirmed))]
    [MapperIgnoreTarget(nameof(PersonalInfoModel.EmailConfirmed))]
    public override partial void Map(ProfileDto source, PersonalInfoModel destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
[MapExtraProperties]
public partial class PersonalInfoModelToUpdateProfileDtoMapper
    : MapperBase<PersonalInfoModel, UpdateProfileDto>
{
    public override partial UpdateProfileDto Map(PersonalInfoModel source);

    public override partial void Map(PersonalInfoModel source, UpdateProfileDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class AuditLogToAuditLogDtoMapper : MapperBase<AuditLog, AuditLogDto>
{
    public override partial AuditLogDto Map(AuditLog source);

    public override partial void Map(AuditLog source, AuditLogDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class UpdateEmailSettingsVmToUpdateEmailSettingsDtoMapper
    : MapperBase<UpdateEmailSettingsVm, UpdateEmailSettingsDto>
{
    public override partial UpdateEmailSettingsDto Map(UpdateEmailSettingsVm source);

    public override partial void Map(
        UpdateEmailSettingsVm source,
        UpdateEmailSettingsDto destination
    );
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class EmailSettingsDtoToUpdateEmailSettingsVmMapper
    : MapperBase<EmailSettingsDto, UpdateEmailSettingsVm>
{
    public override partial UpdateEmailSettingsVm Map(EmailSettingsDto source);

    public override partial void Map(EmailSettingsDto source, UpdateEmailSettingsVm destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class SendTestEmailVMToSendTestEmailInputMapper
    : MapperBase<SendTestEmailVM, SendTestEmailInput>
{
    public override partial SendTestEmailInput Map(SendTestEmailVM source);

    public override partial void Map(SendTestEmailVM source, SendTestEmailInput destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class OrganizationUnitToOrganizationUnitDtoMapper
    : MapperBase<OrganizationUnit, OrganizationUnitDto>
{
    public override partial OrganizationUnitDto Map(OrganizationUnit source);

    public override partial void Map(OrganizationUnit source, OrganizationUnitDto destination);
}
