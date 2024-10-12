using Microsoft.AspNetCore.Authorization;
using Radzen;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.Localization;

namespace Abp.RadzenUI.Components.Pages.Tenant;

public partial class List
{
    protected bool HasManageFeaturesPermission;
    protected string ManageFeaturesPolicyName;

    public List()
    {
        LocalizationResource = typeof(AbpTenantManagementResource);
        ObjectMapperContext = typeof(AbpRadzenUIModule);

        CreatePolicyName = TenantManagementPermissions.Tenants.Create;
        UpdatePolicyName = TenantManagementPermissions.Tenants.Update;
        DeletePolicyName = TenantManagementPermissions.Tenants.Delete;

        ManageFeaturesPolicyName = TenantManagementPermissions.Tenants.ManageFeatures;
    }

    protected override Task<TenantUpdateDto> SetEditDialogModelAsync(TenantDto dto)
    {
        return Task.FromResult(new TenantUpdateDto { Name = dto.Name });
    }

    protected override async Task SetPermissionsAsync()
    {
        await base.SetPermissionsAsync();

        HasManageFeaturesPermission = await AuthorizationService.IsGrantedAsync(
            ManageFeaturesPolicyName
        );
    }

    protected override async Task UpdateGetListInputAsync(LoadDataArgs args)
    {
        GetListInput.Filter = _keyword;
        await base.UpdateGetListInputAsync(args);
    }

    private DialogOptions SetDialogOptions(int heigth = 450, int width = 700)
    {
        return new DialogOptions
        {
            Draggable = true,
            Width = $"{width}px",
            Height = $"{heigth}px",
        };
    }
}
