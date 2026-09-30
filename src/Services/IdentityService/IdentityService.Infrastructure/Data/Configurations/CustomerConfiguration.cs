using IdentityService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdentityService.Infrastructure.Data.CustomerConfiguration;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.FirstName).HasMaxLength(50).IsRequired();

        builder.Property(x => x.LastName).HasMaxLength(50).IsRequired();

        builder.Property(x => x.PhoneNumber).HasMaxLength(10).IsRequired();

        builder.Property(x => x.Country).HasMaxLength(100).IsRequired();
    }
}