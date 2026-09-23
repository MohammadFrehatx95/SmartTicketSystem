using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NotificationService.Domain.Entities;

namespace NotificationService.Infrastructure.Persistence.Configurations
{
    public class ProcessedEventConfiguration : IEntityTypeConfiguration<ProcessedEvent>
    {
        public void Configure(EntityTypeBuilder<ProcessedEvent> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.EventId).IsRequired().HasMaxLength(200);

            builder.HasIndex(x => x.EventId).IsUnique();

            builder.Property(x => x.ProcessedAt).IsRequired();
        }
    }
}