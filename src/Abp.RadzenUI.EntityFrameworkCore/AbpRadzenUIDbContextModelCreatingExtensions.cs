using Abp.RadzenUI.DataDictionaries;
using Abp.RadzenUI.LocalizationTexts;
using Abp.RadzenUI.Messages;
using Microsoft.EntityFrameworkCore;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Abp.RadzenUI.EntityFrameworkCore;

public static class AbpRadzenUIDbContextModelCreatingExtensions
{
    public static void ConfigureAbpRadzenUI(this ModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));

        builder.Entity<DataDictionaryType>(b =>
        {
            b.ToTable(AbpRadzenUIDbProperties.DbTablePrefix + "DataDictionaryTypes", AbpRadzenUIDbProperties.DbSchema);
            b.ConfigureByConvention();
            b.Property(p => p.IsShared).IsRequired().HasDefaultValue(false);
            b.Property(p => p.Code).IsRequired().HasMaxLength(DataDictionaryTypeConsts.MaxCodeLength);
            b.Property(p => p.Name).IsRequired().HasMaxLength(DataDictionaryTypeConsts.MaxNameLength);
            b.Property(p => p.Description).HasMaxLength(DataDictionaryTypeConsts.MaxDescriptionLength);
            b.HasIndex(p => new { p.TenantId, p.Code }).IsUnique();
        });

        builder.Entity<DataDictionaryItem>(b =>
        {
            b.ToTable(AbpRadzenUIDbProperties.DbTablePrefix + "DataDictionaryItems", AbpRadzenUIDbProperties.DbSchema);
            b.ConfigureByConvention();
            b.Property(p => p.DataDictionaryTypeId).IsRequired();
            b.Property(p => p.Code).IsRequired().HasMaxLength(DataDictionaryItemConsts.MaxCodeLength);
            b.Property(p => p.Name).IsRequired().HasMaxLength(DataDictionaryItemConsts.MaxNameLength);
            b.Property(p => p.Description).HasMaxLength(DataDictionaryItemConsts.MaxDescriptionLength);
            b.Property(p => p.Sort).IsRequired();
            b.Property(p => p.IsActive).IsRequired().HasDefaultValue(true);
            b.HasIndex(p => new { p.TenantId, p.DataDictionaryTypeId, p.Code }).IsUnique();
        });

        builder.Entity<LocalizationText>(b =>
        {
            b.ToTable(AbpRadzenUIDbProperties.DbTablePrefix + "LocalizationTexts", AbpRadzenUIDbProperties.DbSchema);
            b.ConfigureByConvention();
            b.Property(p => p.ResourceName).IsRequired().HasMaxLength(LocalizationTextConsts.MaxResourceNameLength);
            b.Property(p => p.CultureName).IsRequired().HasMaxLength(LocalizationTextConsts.MaxCultureNameLength);
            b.Property(p => p.Key).IsRequired().HasMaxLength(LocalizationTextConsts.MaxKeyLength);
            b.Property(p => p.Value).IsRequired().HasColumnType("text");
            b.HasIndex(p => new { p.TenantId, p.ResourceName, p.CultureName, p.Key }).IsUnique();
            b.HasIndex(p => new { p.TenantId, p.ResourceName, p.CultureName });
        });

        builder.Entity<UserMessage>(b =>
        {
            b.ToTable(AbpRadzenUIDbProperties.DbTablePrefix + "UserMessages", AbpRadzenUIDbProperties.DbSchema);
            b.ConfigureByConvention();
            b.Property(p => p.UserId).IsRequired();
            b.Property(p => p.Title).IsRequired().HasMaxLength(MessageConsts.MaxTitleLength);
            b.Property(p => p.Content).IsRequired().HasColumnType("text");
            b.Property(p => p.MessageType).IsRequired().HasMaxLength(MessageConsts.MaxMessageTypeLength);
            b.Property(p => p.IsRead).IsRequired().HasDefaultValue(false);
            b.HasIndex(p => new { p.TenantId, p.UserId, p.CreationTime });
            b.HasIndex(p => new { p.TenantId, p.UserId, p.IsRead, p.CreationTime });
            b.HasIndex(p => new { p.TenantId, p.UserId, p.MessageType, p.CreationTime });
        });
    }
}