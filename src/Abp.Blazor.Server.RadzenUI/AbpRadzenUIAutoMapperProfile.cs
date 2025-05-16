using Abp.RadzenUI.Application.Contracts.AuditLogs;
using Abp.RadzenUI.Models;
using AutoMapper;
using Volo.Abp.Account;
using Volo.Abp.AuditLogging;
using Volo.Abp.AutoMapper;
using Volo.Abp.SettingManagement;

namespace Abp.RadzenUI;

public class AbpRadzenUIAutoMapperProfile : Profile
{
    public AbpRadzenUIAutoMapperProfile()
    {
        CreateMap<ProfileDto, PersonalInfoModel>()
            .MapExtraProperties()
            .Ignore(x => x.PhoneNumberConfirmed)
            .Ignore(x => x.EmailConfirmed);

        CreateMap<PersonalInfoModel, UpdateProfileDto>().MapExtraProperties();
        CreateMap<AuditLog, AuditLogDto>();

        CreateMap<UpdateEmailSettingsVM, UpdateEmailSettingsDto>();
        CreateMap<EmailSettingsDto, UpdateEmailSettingsVM>();
        CreateMap<SendTestEmailVM, SendTestEmailInput>();
    }
}
