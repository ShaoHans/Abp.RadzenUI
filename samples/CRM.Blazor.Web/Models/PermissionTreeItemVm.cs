using Volo.Abp.PermissionManagement;

namespace CRM.Blazor.Web.Models;

public class PermissionTreeItemVm
{
    public int Depth { get; set; }

    public PermissionGrantInfoDto Permission { get; set; }

    public PermissionTreeItemVm? Parent { get; set; }

    public List<PermissionTreeItemVm> Children { get; set; } = [];

    public PermissionTreeItemVm(int depth, PermissionGrantInfoDto permission)
    {
        Depth = depth;
        Permission = permission;
    }

    public void SetParent(PermissionTreeItemVm? parent)
    {
        Parent = parent;
    }
}
