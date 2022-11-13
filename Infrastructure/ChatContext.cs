using Microsoft.EntityFrameworkCore;
using StatusApp_Server.Domain;

namespace StatusApp_Server.Infrastructure
{
    public class ChatContext : DbContext
    {
        public DbSet<Message> Messages { get; set; }
        public DbSet<User> Users { get; set; }

        public string? ConnectionString { get; }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        //    optionsBuilder.UseNpgsql(ConnectionString).EnableSensitiveDataLogging();

        public ChatContext(DbContextOptions<ChatContext> options) : base(options) { }

        public ChatContext() { }
    }
}
