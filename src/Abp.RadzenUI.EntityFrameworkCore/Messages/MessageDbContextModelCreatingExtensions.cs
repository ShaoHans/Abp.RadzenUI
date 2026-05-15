using Microsoft.EntityFrameworkCore;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Abp.RadzenUI.Messages;

public static class MessageDbContextModelCreatingExtensions
{
    public static void ConfigureMessages(this ModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));

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