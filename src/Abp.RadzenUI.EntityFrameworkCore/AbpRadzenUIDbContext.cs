using Abp.RadzenUI.DataDictionaries;
using Abp.RadzenUI.Messages;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Abp.RadzenUI.EntityFrameworkCore;

public interface IAbpRadzenUIDbContext : IEfCoreDbContext
{
    DbSet<DataDictionaryType> DataDictionaryTypes { get; set; }

    DbSet<DataDictionaryItem> DataDictionaryItems { get; set; }

    DbSet<UserMessage> UserMessages { get; set; }
}

[ConnectionStringName("Default")]
public class AbpRadzenUIDbContext(DbContextOptions<AbpRadzenUIDbContext> options)
    : AbpDbContext<AbpRadzenUIDbContext>(options), IAbpRadzenUIDbContext
{
    public DbSet<DataDictionaryType> DataDictionaryTypes { get; set; }

    public DbSet<DataDictionaryItem> DataDictionaryItems { get; set; }

    public DbSet<UserMessage> UserMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ConfigureAbpRadzenUI();
    }
}