using Microsoft.EntityFrameworkCore;
using SadhanaApp.Domain;
using System.Collections.Generic;
using System.Reflection.Emit;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<ChantingRecord> ChantingRecords { get; set; }
    public DbSet<ServiceType> ServiceTypes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasMany(u => u.Devotees)
            .WithOne(s => s.ShikshaGuru)
            .HasForeignKey(s => s.ShikshaGuruId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false); // This is the line making ShikshaGuruId nullable

        modelBuilder.Entity<ChantingRecord>()
            .HasKey(c => c.Id);

        modelBuilder.Entity<ServiceType>().HasData(
            new ServiceType { ServiceTypeId = 1, ServiceName = "Cleaning Temple" },
            new ServiceType { ServiceTypeId = 2, ServiceName = "Garlands" },
            new ServiceType { ServiceTypeId = 3, ServiceName = "Cooking" },
            new ServiceType { ServiceTypeId = 4, ServiceName = "Serving Prasadam" },
            new ServiceType { ServiceTypeId = 5, ServiceName = "Book Distribution" },
            new ServiceType { ServiceTypeId = 6, ServiceName = "Giving Lecture" },
            new ServiceType { ServiceTypeId = 7, ServiceName = "Deity Worship" },
            new ServiceType { ServiceTypeId = 8, ServiceName = "Voice Program Lecture" },
            new ServiceType { ServiceTypeId = 9, ServiceName = "Voice Program Service" },
            new ServiceType { ServiceTypeId = 10, ServiceName = "Digital Service" }
            );
    }
}
