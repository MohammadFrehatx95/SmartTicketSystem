using Microsoft.EntityFrameworkCore;
using NotificationService.Domain.Entities;

namespace NotificationService.Infrastructure.Persistence
{
    public class NotificationDbContext : DbContext
    {
        public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base(options)
        {
        }

        public DbSet<Notification> Notifications { get; set; }
        public DbSet<ProcessedEvent> ProcessedEvents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(NotificationDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}