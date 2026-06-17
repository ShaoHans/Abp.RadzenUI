using Abp.RadzenUI.DataDictionaries;
using Abp.RadzenUI.EntityFrameworkCore;
using Abp.RadzenUI.Messages;
using CRM.Products;
using CRM.Operations;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.BlobStoring.Database.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.EntityFrameworkCore;

namespace CRM.EntityFrameworkCore;

[ReplaceDbContext(typeof(IIdentityDbContext))]
[ReplaceDbContext(typeof(ITenantManagementDbContext))]
[ReplaceDbContext(typeof(IAbpRadzenUIDbContext))]
[ConnectionStringName("Default")]
public class CRMDbContext(DbContextOptions<CRMDbContext> options)
    : AbpDbContext<CRMDbContext>(options),
        ITenantManagementDbContext,
        IIdentityDbContext,
        IAbpRadzenUIDbContext
{
    /* Add DbSet properties for your Aggregate Roots / Entities here. */


    #region Entities from the modules

    /* Notice: We only implemented IIdentityProDbContext and ISaasDbContext
     * and replaced them for this DbContext. This allows you to perform JOIN
     * queries for the entities of these modules over the repositories easily. You
     * typically don't need that for other modules. But, if you need, you can
     * implement the DbContext interface of the needed module and use ReplaceDbContext
     * attribute just like IIdentityProDbContext and ISaasDbContext.
     *
     * More info: Replacing a DbContext of a module ensures that the related module
     * uses this DbContext on runtime. Otherwise, it will use its own DbContext class.
     */

    // Identity
    public DbSet<IdentityUser> Users { get; set; }
    public DbSet<IdentityRole> Roles { get; set; }
    public DbSet<IdentityClaimType> ClaimTypes { get; set; }
    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
    public DbSet<IdentityLinkUser> LinkUsers { get; set; }
    public DbSet<IdentityUserDelegation> UserDelegations { get; set; }
    public DbSet<IdentitySession> Sessions { get; set; }

    // Tenant Management
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<TenantConnectionString> TenantConnectionStrings { get; set; }

    // Business Modules
    public DbSet<Product> Products { get; set; }
    public DbSet<WorkOrder> WorkOrders { get; set; }
    public DbSet<WorkOrderEvent> WorkOrderEvents { get; set; }
    public DbSet<OperationAsset> OperationAssets { get; set; }
    public DbSet<OperationShift> OperationShifts { get; set; }

    // Data Dictionary
    public DbSet<DataDictionaryType> DataDictionaryTypes { get; set; }
    public DbSet<DataDictionaryItem> DataDictionaryItems { get; set; }

    // User Messages
    public DbSet<UserMessage> UserMessages { get; set; }

    #endregion

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        /* Include modules to your migration db context */

        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureBackgroundJobs();
        builder.ConfigureAuditLogging();
        builder.ConfigureFeatureManagement();
        builder.ConfigureIdentity();
        builder.ConfigureOpenIddict();
        builder.ConfigureTenantManagement();
        builder.ConfigureBlobStoring();
        builder.ConfigureAbpRadzenUI();

        /* Configure your own tables/entities inside here */

        //builder.Entity<YourEntity>(b =>
        //{
        //    b.ToTable(CRMConsts.DbTablePrefix + "YourEntities", CRMConsts.DbSchema);
        //    b.ConfigureByConvention(); //auto configure for the base class props
        //    //...
        //});

        builder.Entity<Product>(b =>
        {
            b.ToTable("Products", CRMConsts.DbSchema);
            b.Property(p => p.Code).IsRequired().HasMaxLength(ProductConsts.MaxCodeLength);
            b.Property(p => p.Name).IsRequired().HasMaxLength(ProductConsts.MaxNameLength);
            b.Property(p => p.Price).IsRequired();
            b.Property(p => p.StockCount).IsRequired();
            b.Property(p => p.ImagePath).HasMaxLength(ProductConsts.MaxImagePathLength);
            b.Property(p => p.Status).HasConversion<string>().HasMaxLength(100);
        });

        builder.Entity<WorkOrder>(b =>
        {
            b.ToTable("OperationWorkOrders", CRMConsts.DbSchema);
            b.Property(p => p.Code).IsRequired().HasMaxLength(OperationConsts.MaxCodeLength);
            b.Property(p => p.Title).IsRequired().HasMaxLength(OperationConsts.MaxTitleLength);
            b.Property(p => p.Type).HasConversion<string>().HasMaxLength(64);
            b.Property(p => p.Status).HasConversion<string>().HasMaxLength(64);
            b.Property(p => p.Priority).HasConversion<string>().HasMaxLength(64);
            b.Property(p => p.OwnerName).IsRequired().HasMaxLength(OperationConsts.MaxOwnerLength);
            b.Property(p => p.Location).IsRequired().HasMaxLength(OperationConsts.MaxLocationLength);
            b.Property(p => p.Description).HasMaxLength(OperationConsts.MaxDescriptionLength);
        });

        builder.Entity<WorkOrderEvent>(b =>
        {
            b.ToTable("OperationWorkOrderEvents", CRMConsts.DbSchema);
            b.Property(p => p.Status).HasConversion<string>().HasMaxLength(64);
            b.Property(p => p.OperatorName).IsRequired().HasMaxLength(OperationConsts.MaxOwnerLength);
            b.Property(p => p.Summary).IsRequired().HasMaxLength(OperationConsts.MaxSummaryLength);
            b.HasIndex(p => p.WorkOrderId);
        });

        builder.Entity<OperationAsset>(b =>
        {
            b.ToTable("OperationAssets", CRMConsts.DbSchema);
            b.Property(p => p.Code).IsRequired().HasMaxLength(OperationConsts.MaxCodeLength);
            b.Property(p => p.Name).IsRequired().HasMaxLength(OperationConsts.MaxNameLength);
            b.Property(p => p.Category).IsRequired().HasMaxLength(OperationConsts.MaxNameLength);
            b.Property(p => p.Location).IsRequired().HasMaxLength(OperationConsts.MaxLocationLength);
            b.Property(p => p.Status).HasConversion<string>().HasMaxLength(64);
        });

        builder.Entity<OperationShift>(b =>
        {
            b.ToTable("OperationShifts", CRMConsts.DbSchema);
            b.Property(p => p.ShiftType).HasConversion<string>().HasMaxLength(64);
            b.Property(p => p.OwnerName).IsRequired().HasMaxLength(OperationConsts.MaxOwnerLength);
            b.Property(p => p.Responsibility).IsRequired().HasMaxLength(OperationConsts.MaxSummaryLength);
        });
    }
}
