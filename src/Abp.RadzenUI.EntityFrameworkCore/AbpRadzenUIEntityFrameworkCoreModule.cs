using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace Abp.RadzenUI.EntityFrameworkCore;

[DependsOn(typeof(AbpRadzenUIDomainModule), typeof(AbpEntityFrameworkCoreModule))]
public class AbpRadzenUIEntityFrameworkCoreModule : AbpModule
{
	public override void ConfigureServices(ServiceConfigurationContext context)
	{
		context.Services.AddAbpDbContext<AbpRadzenUIDbContext>(options =>
		{
			options.AddDefaultRepositories();
		});
	}
}