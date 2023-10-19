using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configuration;

public class SignalRConnectionConfiguration : IEntityTypeConfiguration<SignalRConnection>
{
    public void Configure(EntityTypeBuilder<SignalRConnection> signalRConnection)
    {
        signalRConnection.HasKey(t => t.ConnectionId);

        signalRConnection.HasOne(s => s.StatusUser).WithMany().HasForeignKey(s => s.UserName);
    }
}
