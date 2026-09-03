using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketService.Domain.Entities;

namespace TicketService.Infrastructure.Data.Configurations
{
    public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
    {
        public void Configure(EntityTypeBuilder<OutboxMessage> builder)
        {
            builder.ToTable("OutboxMessages");

            builder.HasIndex(x => x.EventKey).IsUnique();

            builder.HasKey(x => x.Id);

            builder.Property(x => x.EventKey).IsRequired().HasMaxLength(200);

            builder.Property(x => x.EventType).IsRequired().HasMaxLength(200);

            builder.Property(x => x.Payload).IsRequired();
        }
    }
}
