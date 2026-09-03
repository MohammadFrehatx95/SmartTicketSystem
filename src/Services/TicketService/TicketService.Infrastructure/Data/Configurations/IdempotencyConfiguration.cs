using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketService.Domain.Entities;

namespace TicketService.Infrastructure.Data.Configurations
{
    public class IdempotencyConfiguration : IEntityTypeConfiguration<IdempotencyRecord>
    {
        public void Configure(EntityTypeBuilder<IdempotencyRecord> builder)
        {
            builder.ToTable("IdempotencyKeys");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.IdempotencyKey).IsUnique();

            builder.Property(x => x.IdempotencyKey).IsRequired().HasMaxLength(200);

            builder.Property(x => x.RequestHash).IsRequired().HasMaxLength(500);

            builder.Property(x => x.ResponseBody).IsRequired();
        }
    }
}
