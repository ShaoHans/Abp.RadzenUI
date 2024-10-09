using System.Threading.Tasks;

namespace CRM.Data;

public interface ICRMDbSchemaMigrator
{
    Task MigrateAsync();
}
