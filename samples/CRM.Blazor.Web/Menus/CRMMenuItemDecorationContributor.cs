using Abp.RadzenUI.Application.Contracts.Messages;
using Abp.RadzenUI.Infrastructure.Navigation;
using Radzen;
using static Abp.RadzenUI.Menus.RadzenUI;

namespace CRM.Blazor.Web.Menus;

public class CRMMenuItemDecorationContributor(
    ILogger<CRMMenuItemDecorationContributor> logger,
    IMessageAppService messageAppService
) : IMenuItemDecorationContributor
{
    public async Task ConfigureAsync(MenuItemDecorationContext context)
    {
        try
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
                case MessageMenuNames.Default:
                    context.RefreshKeys.Add(MenuItemDecorationRefreshKeys.UnreadMessages);
                    var unreadCount = await messageAppService.GetUnreadCountAsync();
                    AddCountBadge(context, unreadCount, BadgeStyle.Danger, 1);
                    break;

                case CRMMenus.Operations:
                    context.Badges.Add(
                        new MenuItemBadgeDefinition
                        {
                            Text = "New",
                            BadgeStyle = BadgeStyle.Success,
                            Order = 1,
                        }
                    );
                    break;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Load menu badge failed，menu item name：{MenuItemName}", context.MenuItem.Name);
        }
    }

    private static void AddCountBadge(
        MenuItemDecorationContext context,
        long count,
        BadgeStyle badgeStyle,
        int order
    )
    {
        if (count <= 0)
        {
            return;
        }

        context.Badges.Add(
            new MenuItemBadgeDefinition
            {
                Text = count > 99 ? "99+" : count.ToString(),
                BadgeStyle = badgeStyle,
                Order = order,
            }
        );
    }
}
