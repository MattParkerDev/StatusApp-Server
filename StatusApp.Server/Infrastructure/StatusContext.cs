using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StatusApp.Server.Domain;

namespace StatusApp.Server.Infrastructure;

public class StatusContext : IdentityUserContext<User>
{
    public StatusContext(DbContextOptions<StatusContext> options)
        : base(options) { }

    // https://learn.microsoft.com/en-us/ef/core/miscellaneous/nullable-reference-types#dbcontext-and-dbset
    public virtual DbSet<Message> Messages { get; set; } = null!;
    public virtual DbSet<Friendship> Friendships { get; set; } = null!;
    public virtual DbSet<Connection> Connections { get; set; } = null!;
    public virtual DbSet<StatusUser> StatusUsers { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
