using Microsoft.EntityFrameworkCore;

namespace Abp.RadzenUI.Messages;

public interface IMessageDbContext
{
    DbSet<UserMessage> UserMessages { get; set; }
}