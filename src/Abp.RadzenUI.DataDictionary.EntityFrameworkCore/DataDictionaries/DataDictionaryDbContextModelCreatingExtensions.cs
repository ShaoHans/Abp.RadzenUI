using Microsoft.EntityFrameworkCore;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Abp.RadzenUI.DataDictionaries;

public static class DataDictionaryDbContextModelCreatingExtensions
{
    public static void ConfigureDataDictionary(this ModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));

        builder.Entity<DataDictionaryType>(b =>
        {
            b.ToTable(DataDictionaryDbProperties.DbTablePrefix + "DataDictionaryTypes", DataDictionaryDbProperties.DbSchema);
            b.ConfigureByConvention();
            b.Property(p => p.Code).IsRequired().HasMaxLength(DataDictionaryTypeConsts.MaxCodeLength);
            b.Property(p => p.Name).IsRequired().HasMaxLength(DataDictionaryTypeConsts.MaxNameLength);
            b.Property(p => p.Description).HasMaxLength(DataDictionaryTypeConsts.MaxDescriptionLength);
            b.HasIndex(p => new { p.TenantId, p.Code }).IsUnique();
        });

        builder.Entity<DataDictionaryItem>(b =>
        {
            b.ToTable(DataDictionaryDbProperties.DbTablePrefix + "DataDictionaryItems", DataDictionaryDbProperties.DbSchema);
            b.ConfigureByConvention();
            b.Property(p => p.DataDictionaryTypeId).IsRequired();
            b.Property(p => p.Code).IsRequired().HasMaxLength(DataDictionaryItemConsts.MaxCodeLength);
            b.Property(p => p.Name).IsRequired().HasMaxLength(DataDictionaryItemConsts.MaxNameLength);
            b.Property(p => p.Description).HasMaxLength(DataDictionaryItemConsts.MaxDescriptionLength);
            b.Property(p => p.Sort).IsRequired();
            b.Property(p => p.IsActive).IsRequired().HasDefaultValue(true);
            b.HasIndex(p => new { p.TenantId, p.DataDictionaryTypeId, p.Code }).IsUnique();
        });
    }
}
