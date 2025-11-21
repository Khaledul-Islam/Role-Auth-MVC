using Role_Auth_MVC.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Role_Auth_MVC.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Fluent API Configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Username).IsRequired().HasMaxLength(50);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(100);
            entity.Property(u => u.PasswordHash).IsRequired();
            entity.Property(u => u.FullName).IsRequired().HasMaxLength(100);
            entity.HasIndex(u => u.Username).IsUnique();
            entity.HasIndex(u => u.Email).IsUnique();
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.Property(r => r.Name).IsRequired().HasMaxLength(50);
            entity.HasIndex(r => r.Name).IsUnique();
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(ur => new { ur.UserId, ur.RoleId });

            entity.HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Seed Data
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Seed Roles
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Name = "Admin", Description = "System Administrator" },
            new Role { Id = 2, Name = "Customer", Description = "Regular Customer" }
        );

        // Seed Users (password: "Test@123" for all)
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Username = "admin",
                Email = "admin@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Test@123"),
                FullName = "Admin User",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new User
            {
                Id = 2,
                Username = "customer",
                Email = "customer@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Test@123"),
                FullName = "Customer User",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        );

        // Seed UserRoles
        modelBuilder.Entity<UserRole>().HasData(
            new UserRole { UserId = 1, RoleId = 1 },
            new UserRole { UserId = 2, RoleId = 2 }
        );
    }
}