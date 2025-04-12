using Volo.Abp.PermissionManagement;

namespace Abp.RadzenUI.Models;

public class PermissionTreeItemVm(int depth, PermissionGrantInfoDto permission)
{
    public int Depth { get; set; } = depth;

    public PermissionGrantInfoDto Permission { get; set; } = permission;

    public PermissionTreeItemVm? Parent { get; set; }

    public List<PermissionTreeItemVm> Children { get; set; } = [];

    public void SetParent(PermissionTreeItemVm? parent)
    {
        Parent = parent;
    }
}
