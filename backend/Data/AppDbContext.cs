using backend.Model.Database;
using backend.Models;
using Microsoft.EntityFrameworkCore;


namespace backend.Data;

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Attendance> Attendances { get; set; }

        public DbSet<Player> Players { get; set; }

        public DbSet<CashIn> CashIns { get; set; }

        public DbSet<CashOut> CashOuts { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

                // Attendance -> Player
                modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Player)
                .WithMany(u => u.Attendances)
                .HasForeignKey(a => a.PlayerId);

                modelBuilder.Entity<Attendance>()
                    .HasIndex(a => new { a.PlayerId, a.AttendanceDate })
                    .IsUnique();


                // CashIn -> Player
                modelBuilder.Entity<CashIn>()
                    .HasOne(c => c.Player)
                    .WithMany(p => p.CashIns)
                    .HasForeignKey(c => c.PlayerId);


                // =====================================================
                // Audit Log
                // =====================================================

                modelBuilder.Entity<AuditLog>(entity =>
                {
                    entity.ToTable("AuditLogs");

                    entity.HasKey(a => a.Id);

                    entity.Property(a => a.Action)
                        .HasMaxLength(50)
                        .IsRequired();

                    entity.Property(a => a.EntityName)
                        .HasMaxLength(100)
                        .IsRequired();

                    entity.Property(a => a.EntityId)
                        .HasMaxLength(100);

                    entity.Property(a => a.Timestamp)
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("SYSUTCDATETIME()");

                    entity.HasOne(a => a.User)
                        .WithMany()
                        .HasForeignKey(a => a.UserId)
                        .OnDelete(DeleteBehavior.SetNull);
                });


                // =====================================================
                // Role
                // =====================================================

                modelBuilder.Entity<User>()
                    .HasOne(u => u.Role)
                    .WithMany(r => r.Users)
                    .HasForeignKey(u => u.RoleId);


        }

    }
