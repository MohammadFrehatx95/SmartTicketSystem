using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using TicketService.Domain.Entities;

namespace TicketService.Infrastructure.Data
{
    public class TicketDbContext : DbContext
    {
        public TicketDbContext(DbContextOptions<TicketDbContext> options) : base(options)
        {
            
        }

        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Agent> Agents { get; set; }
        public DbSet<AssignmentAttempt> AssignmentAttempts { get; set; }
        public DbSet<IdempotencyRecord> IdempotencyRecords { get; set; }
        public DbSet<OutboxMessage> OutboxMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(TicketDbContext).Assembly);
        }
    }
}
