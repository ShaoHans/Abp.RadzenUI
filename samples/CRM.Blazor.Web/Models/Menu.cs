namespace CRM.Blazor.Web.Models;

public class Menu
{
    public bool New { get; set; }
    public bool Updated { get; set; }
    public string Name { get; set; } = default!;
    public string? Icon { get; set; }
    public string Path { get; set; } = default!;
    public string? Title { get; set; }
    public string? Description { get; set; }
    public bool Expanded { get; set; }
    public IEnumerable<Menu> Children { get; set; } = [];
    public IEnumerable<string> Tags { get; set; } = [];
}
