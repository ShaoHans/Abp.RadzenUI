using Abp.RadzenUI.Features.Avatar;
using Abp.RadzenUI.Models;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Account;
using Volo.Abp.Mapperly;
using Volo.Abp.ObjectExtending;
using Volo.Abp.SettingManagement;

namespace Abp.RadzenUI;

// PersonalInfoModel has no extension property definitions of its own; it mirrors
// ProfileDto / UpdateProfileDto. ABP's default pair check would therefore drop every
// extension property, so definition checks are disabled on both directions.
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
[MapExtraProperties(
    DefinitionChecks = MappingPropertyDefinitionChecks.None,
    IgnoredProperties = [AvatarConsts.ExtraPropertyName]
)]
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
[MapExtraProperties(
    DefinitionChecks = MappingPropertyDefinitionChecks.None,
    IgnoredProperties = [AvatarConsts.ExtraPropertyName]
)]
public partial class PersonalInfoModelToUpdateProfileDtoMapper
    : MapperBase<PersonalInfoModel, UpdateProfileDto>
{
    public override partial UpdateProfileDto Map(PersonalInfoModel source);

    public override partial void Map(PersonalInfoModel source, UpdateProfileDto destination);
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
