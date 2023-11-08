using HTML_Parser.Models.Entities;
using Microsoft.EntityFrameworkCore;

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

    public virtual DbSet<Tv> Tvs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=LAPTOP-4AHQIJI5\\SQLEXPRESS;Database=AoIS;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tv>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
