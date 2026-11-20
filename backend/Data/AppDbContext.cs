using backend.Models;
using Microsoft.EntityFrameworkCore;


namespace backend.Data;

public class AppDbContext : DbContext {

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

    public DbSet<PlayerMembership> PlayerMemberships { get; set; }

    public DbSet<SidebarMenu> SidebarMenus { get; set; }

    public DbSet<RoleSidebarMenu> RoleSidebarMenus { get; set; }

    public DbSet<UserSidebarMenu> UserSidebarMenus { get; set; }

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

            modelBuilder.Entity<User>()
                .HasOne(u => u.Player)
                .WithOne(p => p.User)
                .HasForeignKey<User>(u => u.PlayerId)
                .OnDelete(DeleteBehavior.SetNull);

            // =====================================================
            // MemberShip
            // =====================================================
            // Player -> Membership
            modelBuilder.Entity<PlayerMembership>()
                    .HasOne(m => m.Player)
                    .WithMany(p => p.Memberships)
                    .HasForeignKey(m => m.PlayerId)
                    .OnDelete(DeleteBehavior.Restrict);

            // Membership -> CashIn
            modelBuilder.Entity<CashIn>()
                .HasOne(c => c.Membership)
                .WithOne(m => m.CashIn)
                .HasForeignKey<CashIn>(c => c.MembershipId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CashIn>(entity =>
            {
                entity.Property(c => c.Amount).HasColumnType("decimal(18,2)");
                entity.HasIndex(c => new { c.PlayerId, c.CoverageEndDate });
            });

            // Membership configuration
            modelBuilder.Entity<PlayerMembership>(entity =>
            {
                entity.ToTable("PlayerMemberships");

                entity.HasKey(m => m.Id);

                entity.Property(m => m.MonthlyFee)
                    .HasColumnType("decimal(18,2)");

                entity.Property(m => m.OriginalAmount)
                    .HasColumnType("decimal(18,2)");

                entity.Property(m => m.DiscountAmount)
                    .HasColumnType("decimal(18,2)");

                entity.Property(m => m.FinalAmount)
                    .HasColumnType("decimal(18,2)");

                entity.Property(m => m.Status)
                    .HasMaxLength(20)
                    .IsRequired();

                entity.HasIndex(m => m.PlayerId);

                entity.HasIndex(m => new { m.PlayerId, m.EndDate });
            });


        // =====================================================
        // SideBar menu
        // =====================================================

        // Sidebar Menu Configuration

        modelBuilder.Entity<SidebarMenu>(entity =>
        {
            entity.ToTable("SidebarMenus");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Title)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.Route)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(x => x.Icon)
                .HasMaxLength(150);

            entity.HasOne(x => x.Parent)
                .WithMany(x => x.Children)
                .HasForeignKey(x => x.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(x => new { x.ParentId, x.DisplayOrder });
        });

        //modelBuilder.Entity<SidebarMenu>(entity =>
        //{
        //    entity.HasKey(x => x.Id);

        //    entity.Property(x => x.Title)
        //        .HasMaxLength(100)
        //        .IsRequired();

        //    entity.Property(x => x.Route)
        //        .HasMaxLength(200)
        //        .IsRequired();

        //    entity.Property(x => x.Icon)
        //        .HasMaxLength(150);

        //    entity.HasOne(x => x.Parent)
        //        .WithMany(x => x.Children)
        //        .HasForeignKey(x => x.ParentId)
        //        .OnDelete(DeleteBehavior.Restrict);

        //    entity.HasIndex(x => new { x.ParentId, x.DisplayOrder });
        //});

        // Role Sidebar Menu Configuration

        modelBuilder.Entity<RoleSidebarMenu>(entity =>
        {
            entity.ToTable("RoleSidebarMenus");

            entity.HasKey(x => x.Id);

            entity.HasOne(x => x.Role)
                .WithMany()
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(x => x.SidebarMenu)
                .WithMany(x => x.RoleSidebarMenus)
                .HasForeignKey(x => x.SidebarMenuId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(x => new { x.RoleId, x.SidebarMenuId })
                .IsUnique();
        });

        modelBuilder.Entity<UserSidebarMenu>(entity =>
        {
            entity.ToTable("UserSidebarMenus");
            entity.HasKey(x => x.Id);
            entity.HasOne(x => x.User)
                .WithMany(x => x.SidebarMenuAssignments)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(x => x.SidebarMenu)
                .WithMany(x => x.UserSidebarMenus)
                .HasForeignKey(x => x.SidebarMenuId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(x => new { x.UserId, x.SidebarMenuId }).IsUnique();
        });
        //modelBuilder.Entity<RoleSidebarMenu>(entity =>
        //{
        //    entity.HasKey(x => x.Id);

        //    entity.HasOne(x => x.Role)
        //        .WithMany()
        //        .HasForeignKey(x => x.RoleId)
        //        .OnDelete(DeleteBehavior.NoAction);

        //    entity.HasOne(x => x.SidebarMenu)
        //        .WithMany(x => x.RoleSidebarMenus)
        //        .HasForeignKey(x => x.SidebarMenuId)
        //        .OnDelete(DeleteBehavior.Cascade);

        //    entity.HasIndex(x => new { x.RoleId, x.SidebarMenuId })
        //        .IsUnique();
        //});

    }

}
