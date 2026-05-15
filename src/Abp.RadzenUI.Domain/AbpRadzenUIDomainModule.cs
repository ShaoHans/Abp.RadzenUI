using Volo.Abp.Modularity;

namespace Abp.RadzenUI;

[DependsOn(typeof(AbpRadzenUIDomainSharedModule))]
public class AbpRadzenUIDomainModule : AbpModule
{
}