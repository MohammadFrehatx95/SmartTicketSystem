using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketService.Domain.Entities;

namespace TicketService.Infrastructure.Data.Configurations
{
    public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
    {
        public void Configure(EntityTypeBuilder<Ticket> builder)
        {
            builder.ToTable("Tickets");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title).IsRequired().HasMaxLength(200);

            builder.Property(x => x.Description).IsRequired().HasMaxLength(2000);

            builder.Property(x => x.Category).IsRequired().HasMaxLength(100);

            builder.Property(x => x.AssignmentReason).HasMaxLength(500);

            builder.Property(x => x.AssignmentVersion).IsConcurrencyToken();
        }
    }
}
