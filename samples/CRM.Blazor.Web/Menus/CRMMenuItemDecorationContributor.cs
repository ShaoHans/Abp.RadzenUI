using Abp.RadzenUI.Navigation;
using Radzen;

namespace CRM.Blazor.Web.Menus;

public class CRMMenuItemDecorationContributor : IMenuItemDecorationContributor
{
    public Task ConfigureAsync(MenuItemDecorationContext context)
    {
        switch (context.MenuItem.Name)
        {
            //case CRMMenus.Home:
            //    context.Badges.Add(
            //        new MenuItemBadgeDefinition
            //        {
            //            Text = "12",
            //            BadgeStyle = BadgeStyle.Primary,
            //            Order = 1,
            //        }
            //    );
            //    break;

            //case CRMMenus.Dashboard:
            //    context.Badges.Add(
            //        new MenuItemBadgeDefinition
            //        {
            //            Text = "Updated",
            //            BadgeStyle = BadgeStyle.Info,
            //            Order = 1,
            //        }
            //    );
            //    break;

            case CRMMenus.Product:
                context.Badges.Add(
                    new MenuItemBadgeDefinition
                    {
                        Text = "Pro",
                        BadgeStyle = BadgeStyle.Danger,
                        Order = 1,
                    }
                );
                break;

            case CRMMenus.ProductList:
                context.Badges.Add(
                    new MenuItemBadgeDefinition
                    {
                        Text = "New",
                        BadgeStyle = BadgeStyle.Success,
                        Order = 1,
                    }
                );
                context.Badges.Add(
                    new MenuItemBadgeDefinition
                    {
                        Text = "8",
                        BadgeStyle = BadgeStyle.Primary,
                        Order = 2,
                    }
                );
                break;
        }

        return Task.CompletedTask;
    }
}