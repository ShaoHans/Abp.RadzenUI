using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Abp.RadzenUI.DataDictionaries;

[ConnectionStringName("Default")]
public interface IDataDictionaryDbContext : IEfCoreDbContext
{
    DbSet<DataDictionaryType> DataDictionaryTypes { get; set; }

    DbSet<DataDictionaryItem> DataDictionaryItems { get; set; }
}
