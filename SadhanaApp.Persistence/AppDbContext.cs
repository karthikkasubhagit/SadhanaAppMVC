using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<ChantingRecord> WalkingRecords { get; set; }

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
    }
}
