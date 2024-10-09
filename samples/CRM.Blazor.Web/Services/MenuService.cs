using CRM.Blazor.Web.Models;

namespace CRM.Blazor.Web.Services;

public class MenuService
{
    readonly Menu[] allMenus =
    [
        new Menu
        {
            Name = "首页",
            Icon = "home",
            Path = "/",
            Children = []
        },
        new Menu
        {
            Name = "身份认证管理",
            Icon = "security_key",
            Children =
            [
                new Menu
                {
                    Name = "角色",
                    Path = "/role/list",
                    Icon = "safety_check"
                },
                new Menu
                {
                    Name = "用户",
                    Path = "/user/list",
                    Icon = "person"
                },
            ]
        }
    ];

    public IEnumerable<Menu> Menus
    {
        get { return allMenus; }
    }

    public IEnumerable<Menu> Filter(string term)
    {
        if (string.IsNullOrEmpty(term))
            return allMenus;

        bool contains(string value) =>
            value != null && value.Contains(term, StringComparison.OrdinalIgnoreCase);

        bool filter(Menu example) =>
            contains(example.Name) || (example.Tags != null && example.Tags.Any(contains));

        bool deepFilter(Menu example) => filter(example) || example.Children?.Any(filter) == true;

        return Menus
            .Where(category => category.Children.Any(deepFilter) == true || filter(category))
            .Select(category => new Menu
            {
                Name = category.Name,
                Path = category.Path,
                Icon = category.Icon,
                Expanded = true,
                Children = category
                    .Children.Where(deepFilter)
                    .Select(example => new Menu
                    {
                        Name = example.Name,
                        Path = example.Path,
                        Icon = example.Icon,
                        Expanded = true,
                        Children = example.Children
                    })
                    .ToArray()
            })
            .ToList();
    }
}
