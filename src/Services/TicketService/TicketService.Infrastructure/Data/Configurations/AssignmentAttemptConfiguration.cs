using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketService.Domain.Entities;

namespace TicketService.Infrastructure.Data.Configurations
{
    public class AssignmentAttemptConfiguration : IEntityTypeConfiguration<AssignmentAttempt>
    {
        public void Configure(EntityTypeBuilder<AssignmentAttempt> builder)
        {
            builder.ToTable("AssignmentAttempts");

            builder.HasKey(x => x.AttemptId);

            builder.Property(x => x.FailureReason).HasMaxLength(500);

        }
    }
}
