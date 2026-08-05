using Microsoft.EntityFrameworkCore;
using DotnetCRUD.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Security.Claims;
using System.Text.Json;

namespace DotnetCRUD.Data
{
    public class AppDbContext : DbContext
    {
        private readonly IHttpContextAccessor? _httpContextAccessor;

        public AppDbContext(
            DbContextOptions<AppDbContext> options,
            IHttpContextAccessor? httpContextAccessor = null) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public DbSet<User> Users => Set<User>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Vehicle> Vehicles => Set<Vehicle>();
        public DbSet<ServiceCatalog> ServiceCatalogs => Set<ServiceCatalog>();
        public DbSet<Booking> Bookings => Set<Booking>();
        public DbSet<PaymentTransaction> PaymentTransactions => Set<PaymentTransaction>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
         protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var userRoleConverter = new EnumToStringConverter<UserRole>();
            var bookingStatusConverter = new EnumToStringConverter<BookingStatus>();
            var paymentStatusConverter = new EnumToStringConverter<PaymentStatus>();
            var paymentMethodConverter = new EnumToStringConverter<PaymentMethod>();

            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasConversion(userRoleConverter)
                .HasMaxLength(20);

            modelBuilder.Entity<Booking>()
                .Property(b => b.Status)
                .HasConversion(bookingStatusConverter)
                .HasMaxLength(20);

            modelBuilder.Entity<Booking>()
                .Property(b => b.PaymentStatus)
                .HasConversion(paymentStatusConverter)
                .HasMaxLength(20);

            modelBuilder.Entity<PaymentTransaction>()
                .Property(p => p.PaymentStatus)
                .HasConversion(paymentStatusConverter)
                .HasMaxLength(20);

            modelBuilder.Entity<PaymentTransaction>()
                .Property(p => p.PaymentMethod)
                .HasConversion(paymentMethodConverter)
                .HasMaxLength(20);

            modelBuilder.Entity<Vehicle>()
                .HasIndex(v => v.PlateNumber)
                .IsUnique()
                .HasFilter("\"IsDeleted\" = false");

            modelBuilder.Entity<Booking>()
                .Property(b => b.Version)
                .IsConcurrencyToken();

            modelBuilder.Entity<Vehicle>()
                .HasOne(v => v.User)
                .WithMany()
                .HasForeignKey(v => v.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Vehicle)
                .WithMany(v => v.Bookings)
                .HasForeignKey(b => b.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.ServiceCatalog)
                .WithMany(s => s.Bookings)
                .HasForeignKey(b => b.ServiceCatalogId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Mechanic)
                .WithMany()
                .HasForeignKey(b => b.MechanicId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<PaymentTransaction>()
                .HasOne(p => p.Booking)
                .WithMany(b => b.PaymentTransactions)
                .HasForeignKey(p => p.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PaymentTransaction>()
                .Property(p => p.Version)
                .IsConcurrencyToken();

        }

        public override int SaveChanges()
        {
            var auditLogs = PrepareAuditLogs();
            if (auditLogs.Count > 0)
            {
                AuditLogs.AddRange(auditLogs);
            }
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var auditLogs = PrepareAuditLogs();
            if (auditLogs.Count > 0)
            {
                AuditLogs.AddRange(auditLogs);
            }
            return await base.SaveChangesAsync(cancellationToken);
        }

        private List<AuditLog> PrepareAuditLogs()
        {
            ChangeTracker.DetectChanges();

            var user = _httpContextAccessor?.HttpContext?.User;
            var actorUserId = TryGetActorUserId(user);
            var actorRole = user?.FindFirstValue(ClaimTypes.Role);
            var actorIdentity = user?.FindFirstValue(ClaimTypes.Email) ?? user?.Identity?.Name ?? "system";
            var correlationId = _httpContextAccessor?.HttpContext?.TraceIdentifier ?? Guid.NewGuid().ToString("N");
            var occurredAt = DateTime.UtcNow;

            var logs = new List<AuditLog>();

            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is AuditLog || entry.State == EntityState.Unchanged || entry.State == EntityState.Detached)
                {
                    continue;
                }

                var action = entry.State switch
                {
                    EntityState.Added => "CREATE",
                    EntityState.Modified => "UPDATE",
                    EntityState.Deleted => "DELETE",
                    _ => null
                };

                if (action == null)
                {
                    continue;
                }

                var entityName = entry.Metadata.ClrType.Name;
                var entityId = GetPrimaryKeyValue(entry);
                var oldValues = entry.State == EntityState.Added ? null : GetPropertyValues(entry.OriginalValues);
                var newValues = entry.State == EntityState.Deleted ? null : GetPropertyValues(entry.CurrentValues);

                logs.Add(new AuditLog
                {
                    EntityName = entityName,
                    EntityId = entityId,
                    Action = action,
                    OldValues = oldValues,
                    NewValues = newValues,
                    ActorUserId = actorUserId,
                    ActorRole = actorRole,
                    ActorIdentity = actorIdentity,
                    CorrelationId = correlationId,
                    OccurredAt = occurredAt
                });
            }

            return logs;
        }

        private static int? TryGetActorUserId(ClaimsPrincipal? user)
        {
            var nameIdentifier = user?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(nameIdentifier, out var actorId))
            {
                return actorId;
            }

            return null;
        }

        private static string GetPrimaryKeyValue(EntityEntry entry)
        {
            var key = entry.Properties.FirstOrDefault(property => property.Metadata.IsPrimaryKey());
            return key?.CurrentValue?.ToString() ?? "N/A";
        }

        private static string? GetPropertyValues(PropertyValues values)
        {
            var payload = values.Properties.ToDictionary(
                property => property.Name,
                property => values[property.Name]);
            return JsonSerializer.Serialize(payload);
        }
    }
};
