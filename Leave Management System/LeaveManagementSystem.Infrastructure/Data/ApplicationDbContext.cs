using LeaveManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagementSystem.Infrastructure.Data;

/// <summary>
/// Entity Framework Core database context for the application.
/// Following Single Responsibility Principle - handles only database context configuration.
/// </summary>
public class ApplicationDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the ApplicationDbContext class.
    /// </summary>
    /// <param name="options">Database context options</param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// DbSet for Users table
    /// </summary>
    public DbSet<User> Users { get; set; } = null!;

    /// <summary>
    /// DbSet for Companies table
    /// </summary>
    public DbSet<Company> Companies { get; set; } = null!;

    /// <summary>
    /// DbSet for LeaveTypes table
    /// </summary>
    public DbSet<LeaveType> LeaveTypes { get; set; } = null!;

    /// <summary>
    /// DbSet for LeavePolicies table
    /// </summary>
    public DbSet<LeavePolicy> LeavePolicies { get; set; } = null!;

    /// <summary>
    /// DbSet for EmployeeLeaveSettings table
    /// </summary>
    public DbSet<EmployeeLeaveSetting> EmployeeLeaveSettings { get; set; } = null!;

    /// <summary>
    /// DbSet for LeaveRequests table
    /// </summary>
    public DbSet<LeaveRequest> LeaveRequests { get; set; } = null!;

    /// <summary>
    /// DbSet for LeaveBalances table
    /// </summary>
    public DbSet<LeaveBalance> LeaveBalances { get; set; } = null!;

    /// <summary>
    /// DbSet for WorkingHours table
    /// </summary>
    public DbSet<WorkingHours> WorkingHours { get; set; } = null!;

    /// <summary>
    /// DbSet for CompanyUserLimits table
    /// </summary>
    public DbSet<CompanyUserLimit> CompanyUserLimits { get; set; } = null!;

    /// <summary>
    /// DbSet for PricingTiers table
    /// </summary>
    public DbSet<PricingTier> PricingTiers { get; set; } = null!;

    /// <summary>
    /// DbSet for CompanySubscriptions table
    /// </summary>
    public DbSet<CompanySubscription> CompanySubscriptions { get; set; } = null!;

    /// <summary>
    /// DbSet for Payments table
    /// </summary>
    public DbSet<Payment> Payments { get; set; } = null!;

    /// <summary>
    /// DbSet for Roles table
    /// </summary>
    public DbSet<Role> Roles { get; set; } = null!;

    /// <summary>
    /// Configures the model relationships and constraints.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure User entity
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.FullName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.RoleId).IsRequired();
            entity.Property(e => e.Status).IsRequired();

            // Relationship with Role
            entity.HasOne(u => u.Role)
                .WithMany()
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure Role entity
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Name).IsUnique();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.IsActive).IsRequired();
        });

        // Configure Company entity
        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.ContactEmail).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Status).IsRequired();

            // Relationship with Owner
            entity.HasOne(c => c.Owner)
                .WithOne()
                .HasForeignKey<Company>(c => c.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure LeaveType entity
        modelBuilder.Entity<LeaveType>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Unit).IsRequired();
        });

        // Configure LeavePolicy entity
        modelBuilder.Entity<LeavePolicy>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.EntitlementAmount).IsRequired().HasPrecision(18, 2);
            entity.Property(e => e.EntitlementUnit).IsRequired();
            entity.Property(e => e.RenewalPeriod).IsRequired();

            // Relationship with LeaveType
            entity.HasOne(lp => lp.LeaveType)
                .WithMany(lt => lt.LeavePolicies)
                .HasForeignKey(lp => lp.LeaveTypeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure EmployeeLeaveSetting entity
        modelBuilder.Entity<EmployeeLeaveSetting>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CustomEntitlementAmount).IsRequired().HasPrecision(18, 2);
            entity.Property(e => e.CustomEntitlementUnit).IsRequired();

            // Relationship with User
            entity.HasOne(els => els.User)
                .WithMany(u => u.EmployeeLeaveSettings)
                .HasForeignKey(els => els.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relationship with LeaveType
            entity.HasOne(els => els.LeaveType)
                .WithMany(lt => lt.EmployeeLeaveSettings)
                .HasForeignKey(els => els.LeaveTypeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure LeaveRequest entity
        modelBuilder.Entity<LeaveRequest>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.StartDateTime).IsRequired();
            entity.Property(e => e.EndDateTime).IsRequired();
            entity.Property(e => e.DurationAmount).IsRequired().HasPrecision(18, 2);
            entity.Property(e => e.DurationUnit).IsRequired();
            entity.Property(e => e.Status).IsRequired();

            // Relationship with User
            entity.HasOne(lr => lr.User)
                .WithMany(u => u.LeaveRequests)
                .HasForeignKey(lr => lr.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relationship with LeaveType
            entity.HasOne(lr => lr.LeaveType)
                .WithMany(lt => lt.LeaveRequests)
                .HasForeignKey(lr => lr.LeaveTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure LeaveBalance entity
        modelBuilder.Entity<LeaveBalance>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.BalanceAmount).IsRequired().HasPrecision(18, 2);
            entity.Property(e => e.BalanceUnit).IsRequired();

            // Relationship with User
            entity.HasOne(lb => lb.User)
                .WithMany(u => u.LeaveBalances)
                .HasForeignKey(lb => lb.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relationship with LeaveType
            entity.HasOne(lb => lb.LeaveType)
                .WithMany(lt => lt.LeaveBalances)
                .HasForeignKey(lb => lb.LeaveTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure WorkingHours entity
        modelBuilder.Entity<WorkingHours>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Type).IsRequired();
            entity.Property(e => e.Hours).IsRequired().HasPrecision(18, 2);

            // Relationship with User
            entity.HasOne(wh => wh.User)
                .WithMany(u => u.WorkingHours)
                .HasForeignKey(wh => wh.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure CompanyUserLimit entity
        modelBuilder.Entity<CompanyUserLimit>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FreeUserLimit).IsRequired();

            // Relationship with Company (one-to-one)
            entity.HasOne(cul => cul.Company)
                .WithOne(c => c.CompanyUserLimit)
                .HasForeignKey<CompanyUserLimit>(cul => cul.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure PricingTier entity
        modelBuilder.Entity<PricingTier>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.MinUsers).IsRequired();
            entity.Property(e => e.Price).IsRequired().HasPrecision(18, 2);
            entity.Property(e => e.Currency).IsRequired().HasMaxLength(10);
        });

        // Configure CompanySubscription entity
        modelBuilder.Entity<CompanySubscription>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.BillingPeriod).IsRequired();
            entity.Property(e => e.NextBillingDate).IsRequired();
            entity.Property(e => e.Amount).IsRequired().HasPrecision(18, 2);
            entity.Property(e => e.Status).IsRequired();

            // Relationship with Company
            entity.HasOne(cs => cs.Company)
                .WithOne(c => c.CompanySubscription)
                .HasForeignKey<CompanySubscription>(cs => cs.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relationship with PricingTier
            entity.HasOne(cs => cs.CurrentTier)
                .WithMany()
                .HasForeignKey(cs => cs.CurrentTierId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Configure Payment entity
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Amount).IsRequired().HasPrecision(18, 2);
            entity.Property(e => e.Currency).IsRequired().HasMaxLength(10);
            entity.Property(e => e.BillingPeriod).IsRequired().HasMaxLength(50);
            entity.Property(e => e.PaymentMethod).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Status).IsRequired();

            // Relationship with Company
            entity.HasOne(p => p.Company)
                .WithMany(c => c.Payments)
                .HasForeignKey(p => p.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}

