using AutoMapper;
using Abp.RadzenUI.Models;
using Volo.Abp.Account;
using Volo.Abp.AutoMapper;

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
    }
}
