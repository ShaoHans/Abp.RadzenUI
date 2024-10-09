namespace CRM.Blazor.Web.Models;

public class PermissionGroupVm
{
    public string Name { get; set; } = default!;

    public string DisplayName { get; set; } = default!;

    public bool GrantAll
    {
        get { return Expand().All(x => x.Permission.IsGranted); }
        set { Grant(value); }
    }

    public List<PermissionTreeItemVm> TreeItems { get; set; } = [];

    public List<PermissionTreeItemVm> Expand()
    {
        var result = new List<PermissionTreeItemVm>();

        foreach (var item in TreeItems)
        {
            result.Add(item);
            result.AddRange(GetAllChildren(item.Children));
        }

        return result;

        List<PermissionTreeItemVm> GetAllChildren(List<PermissionTreeItemVm> children)
        {
            var result = new List<PermissionTreeItemVm>();
            foreach (var child in children)
            {
                result.Add(child);
                result.AddRange(GetAllChildren(child.Children));
            }
            return result;
        }
    }

    public void Grant(bool isGranted)
    {
        foreach (var item in TreeItems)
        {
            item.Permission.IsGranted = isGranted;
            GrantAllChildren(item.Children, isGranted);
        }

        void GrantAllChildren(List<PermissionTreeItemVm> children, bool isGranted)
        {
            foreach (var child in children)
            {
                child.Permission.IsGranted = isGranted;
                GrantAllChildren(child.Children, isGranted);
            }
        }
    }
}
