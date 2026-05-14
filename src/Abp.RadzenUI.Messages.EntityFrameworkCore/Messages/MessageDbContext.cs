using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Abp.RadzenUI.Messages;

[ConnectionStringName("Default")]
public class MessageDbContext(DbContextOptions<MessageDbContext> options)
    : AbpDbContext<MessageDbContext>(options), IMessageDbContext
{
    public DbSet<UserMessage> UserMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ConfigureMessages();
    }
}