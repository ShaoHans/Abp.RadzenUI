using Abp.RadzenUI.Infrastructure.Navigation;
using Volo.Abp.UI.Navigation;

namespace RadzenUiTemplateNamespace.Menus;

public class RadzenUiTemplateMenuContributor : IMenuContributor
{
    public Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name != StandardMenus.Main)
        {
            return Task.CompletedTask;
        }

        context.Menu.Items.Insert(
            0,
            new ApplicationMenuItem(
                    RadzenUiTemplateMenus.Home,
                    "Home",
                    "/",
                    icon: "home",
                    order: 1)
                .WithIconColor("#2563eb"));

        var administration = context.Menu.GetAdministration();
        administration.Order = 100;

        return Task.CompletedTask;
    }
}
