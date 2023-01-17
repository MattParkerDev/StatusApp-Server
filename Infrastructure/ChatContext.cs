using Microsoft.EntityFrameworkCore;
using StatusApp_Server.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace StatusApp_Server.Infrastructure
{
    public class ChatContext : IdentityUserContext<User>
    {
        public ChatContext(DbContextOptions<ChatContext> options) : base(options) { }

        public DbSet<Message> Messages { get; set; }
        public DbSet<GroupChat> GroupChatRegistry { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Friendship> Friendships { get; set; }
        public DbSet<Connection> Connections { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
