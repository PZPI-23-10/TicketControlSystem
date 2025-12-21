using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using TicketControlSystem.Data.Models;

namespace Ticket_control_system.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Tariff> Tariffs { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Validation> Validations { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<User>()
                .HasMany(u => u.OwnedEvents)
                .WithOne(e => e.Owner)
                .HasForeignKey(e => e.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Event>()
                .HasMany(e => e.Tariffs)
                .WithOne(t => t.Event)
                .HasForeignKey(t => t.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Tariff>()
                .HasMany(t => t.Tickets)
                .WithOne(t => t.Tariff)
                .HasForeignKey(t => t.TariffId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Ticket>()
                .HasMany(t => t.Validations)
                .WithOne(v => v.Ticket)
                .HasForeignKey(v => v.TicketId)
                .OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<Device>(entity =>
            {
                entity.HasOne(d => d.Event)
                    .WithMany() 
                    .HasForeignKey(d => d.EventId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasMany(d => d.Validations)
                    .WithOne(v => v.Device)
                    .HasForeignKey(v => v.DeviceId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateAuditableEntities();
            await base.SaveChangesAsync(cancellationToken);
        }
        
        private void UpdateAuditableEntities()
        {
            IEnumerable<EntityEntry<IBaseTimeControl>> auditableEntities = base
                .ChangeTracker
                .Entries<IBaseTimeControl>();

            foreach (EntityEntry<IBaseTimeControl> entity in auditableEntities)
            {
                DateTime now = DateTime.UtcNow;

                switch (entity.State)
                {
                    case EntityState.Added:
                        entity.Property(e => e.Created).CurrentValue = now;
                        entity.Property(e => e.LastModified).CurrentValue = now;
                        break;

                    case EntityState.Modified:
                        entity.Property(e => e.LastModified).CurrentValue = now;
                        break;
                }
            }
        }
    }
}