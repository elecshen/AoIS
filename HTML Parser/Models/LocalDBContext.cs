using HTML_Parser.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace HTML_Parser.Models;

public partial class LocalDBContext : DbContext
{
    public LocalDBContext()
    {
    }

    public LocalDBContext(DbContextOptions<LocalDBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Laptop> Laptops { get; set; }

    public virtual DbSet<Tv> Tvs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
               .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
               .AddUserSecrets<LocalDBContext>()
               .Build();
        optionsBuilder.UseSqlServer(configuration.GetConnectionString(nameof(LocalDBContext)));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Laptop>(entity =>
        {
            entity.Property(e => e.IdLaptop).HasDefaultValueSql("(newid())");
        });

        modelBuilder.Entity<Tv>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
