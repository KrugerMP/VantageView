using Microsoft.EntityFrameworkCore;
using VantageView.Data.Entities;

namespace VantageView.Data;

/// <summary>
/// Entity Framework Core database context for VantageView.
/// </summary>
public class AppDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppDbContext"/> class.
    /// </summary>
    /// <param name="options">The options to be used by the context.</param>
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Gets the articles entity set.
    /// </summary>
    public DbSet<Article> Articles => Set<Article>();

    /// <summary>
    /// Gets the users entity set (admin portal authentication).
    /// </summary>
    public DbSet<Users> Users => Set<Users>();

    /// <summary>
    /// Gets the article history entity set (snapshots/audit of article changes).
    /// </summary>
    public DbSet<ArticleHistory> ArticleHistory => Set<ArticleHistory>();

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Article>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.Summary).HasMaxLength(500);
            entity.Property(e => e.Author).HasMaxLength(100);
        });

        modelBuilder.Entity<ArticleHistory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.Summary).HasMaxLength(500);
            entity.Property(e => e.Author).HasMaxLength(100);
            entity.HasIndex(e => e.ArticleId);
            // No FK to Article: when an article is deleted, history rows are kept (audit trail).
        });

        modelBuilder.Entity<Users>(entity =>
        {
            entity.HasKey(e => e.UserId);
            entity.Property(e => e.UserName).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(500);
        });
    }
}
