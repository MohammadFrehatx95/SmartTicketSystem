using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketService.Domain.Entities;

namespace TicketService.Infrastructure.Data.Configurations
{
    public class AgentConfiguration : IEntityTypeConfiguration<Agent>
    {
        public void Configure(EntityTypeBuilder<Agent> builder)
        {
            builder.ToTable("Agents");

            builder.HasKey(t => t.Id);

            builder.HasOne(x => x.Department)
                   .WithMany(x => x.Agents)
                   .HasForeignKey(x => x.DepartmentId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(a => a.IdentityUserId).IsUnique();
        }
    }
}

