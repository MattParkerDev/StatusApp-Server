using Microsoft.EntityFrameworkCore;
using StatusApp_Server.Domain;

namespace StatusApp_Server.Infrastructure
{
    public class ChatContext : DbContext
    {
        public DbSet<Message> Messages { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Friendship> Friendships { get; set; }

        public ChatContext(DbContextOptions<ChatContext> options) : base(options) { }

        public ChatContext() { }
    }
}
