using Abp.RadzenUI.Components.Shared;
using Abp.RadzenUI.Infrastructure.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Radzen;
using Volo.Abp.ObjectExtending;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.Localization;

namespace Abp.RadzenUI.Components.Pages.Tenant;

public partial class List
{
    private const string EditKey = "edit";
    private const string FeaturesKey = "features";
    private const string ConnectionStringsKey = "connection-strings";
    private const string DeleteKey = "delete";

    [Inject]
    protected DialogService TenantDialogService { get; set; } = default!;

    protected bool HasManageFeaturesPermission;
    protected string ManageFeaturesPolicyName;
    protected bool HasManageConnectionStringsPermission;
    protected string ManageConnectionStringsPolicyName;
    private IReadOnlyList<ExtraPropertyColumnMeta> _extraColumns = default!;

    public List()
    {
        LocalizationResource = typeof(AbpTenantManagementResource);
        ObjectMapperContext = typeof(AbpRadzenUIModule);

        CreatePolicyName = TenantManagementPermissions.Tenants.Create;
        UpdatePolicyName = TenantManagementPermissions.Tenants.Update;
        DeletePolicyName = TenantManagementPermissions.Tenants.Delete;

        ManageFeaturesPolicyName = TenantManagementPermissions.Tenants.ManageFeatures;
        ManageConnectionStringsPolicyName =
            TenantManagementPermissions.Tenants.ManageConnectionStrings;
    }

    protected override void OnInitialized()
    {
        _extraColumns = RadzenColumnHelper.GetExtraPropertyMetas<TenantDto>();
    }

    protected override Task<TenantUpdateDto> SetEditDialogModelAsync(TenantDto dto)
    {
        var updateDto = new TenantUpdateDto { Name = dto.Name };
        dto.MapExtraPropertiesTo(updateDto);
        return Task.FromResult(updateDto);
    }

    protected override async Task SetPermissionsAsync()
    {
        await base.SetPermissionsAsync();

        HasManageFeaturesPermission = await AuthorizationService.IsGrantedAsync(
            ManageFeaturesPolicyName
        );
        HasManageConnectionStringsPermission = await AuthorizationService.IsGrantedAsync(
            ManageConnectionStringsPolicyName
        );
    }

    /// <summary>
    /// Actions offered for a tenant row, in display order. Availability depends only
    /// on the current user's permissions, not on the row itself.
    /// </summary>
    private IReadOnlyList<RowAction> BuildRowActions()
    {
        var actions = new List<RowAction>();

        if (HasUpdatePermission)
        {
            actions.Add(new RowAction(EditKey, L["Edit"], "edit", IsPrimary: true));
        }

        if (HasManageFeaturesPermission)
        {
            actions.Add(
                new RowAction(FeaturesKey, UL["Permission:ManageFeatures"], "toggle_on")
            );
        }

        if (HasManageConnectionStringsPermission)
        {
            actions.Add(
                new RowAction(ConnectionStringsKey, UL["ConnectionString:Manage"], "link")
            );
        }

        if (HasDeletePermission)
        {
            actions.Add(new RowAction(DeleteKey, L["Delete"], "delete", Colors.Danger));
        }

        return actions;
    }

    private Task HandleRowActionAsync(string key, TenantDto tenant)
    {
        return key switch
        {
            // Not a method group: SetDialogOptions has optional parameters and so
            // does not convert to Func<DialogOptions>.
            EditKey => OpenEditDialogAsync<Edit>(L["Edit"], tenant, () => SetDialogOptions()),
            FeaturesKey => OpenFeaturesDialogAsync(tenant),
            ConnectionStringsKey => OpenConnectionStringsDialogAsync(tenant),
            DeleteKey => OpenDeleteConfirmDialogAsync(
                tenant.Id,
                L["Delete"],
                L["TenantDeletionConfirmationMessage", tenant.Name]
            ),
            _ => Task.CompletedTask,
        };
    }

    private async Task OpenFeaturesDialogAsync(TenantDto tenant)
    {
        await TenantDialogService.OpenAsync<FeatureManagement>(
            UL["Feature:ManageFeatures"],
            new Dictionary<string, object?>
            {
                { "ProviderName", "T" },
                { "ProviderKey", tenant.Id.ToString() },
            },
            new DialogOptions { Draggable = true, Width = "800px" }
        );
    }

    private async Task OpenConnectionStringsDialogAsync(TenantDto tenant)
    {
        await TenantDialogService.OpenAsync<ConnectionStrings>(
            UL["ConnectionString:Manage"],
            new Dictionary<string, object?> { { "TenantId", tenant.Id } },
            new DialogOptions { Draggable = true, Width = "750px" }
        );
    }

    private static DialogOptions SetDialogOptions(int heigth = 450, int width = 700)
    {
        return new DialogOptions
        {
            Draggable = true,
            Width = $"{width}px",
            //Height = $"{heigth}px",
        };
    }
}
