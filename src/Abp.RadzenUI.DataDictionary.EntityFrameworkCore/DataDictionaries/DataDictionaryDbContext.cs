using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Abp.RadzenUI.DataDictionaries;

[ConnectionStringName("Default")]
public class DataDictionaryDbContext(DbContextOptions<DataDictionaryDbContext> options)
    : AbpDbContext<DataDictionaryDbContext>(options), IDataDictionaryDbContext
{
    public DbSet<DataDictionaryType> DataDictionaryTypes { get; set; }

    public DbSet<DataDictionaryItem> DataDictionaryItems { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ConfigureDataDictionary();
    }
}
