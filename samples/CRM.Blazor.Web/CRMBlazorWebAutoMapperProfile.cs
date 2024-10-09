using AutoMapper;
using CRM.Blazor.Web.Models;
using Volo.Abp.Account;
using Volo.Abp.AutoMapper;

namespace CRM.Blazor;

public class CRMBlazorWebAutoMapperProfile : Profile
{
    public CRMBlazorWebAutoMapperProfile()
    {
        CreateMap<ProfileDto, PersonalInfoModel>()
            .MapExtraProperties()
            .Ignore(x => x.PhoneNumberConfirmed)
            .Ignore(x => x.EmailConfirmed);

        CreateMap<PersonalInfoModel, UpdateProfileDto>().MapExtraProperties();
    }
}
