using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ChinaTown.Domain.Entities;
using ChinaTown.Domain.Enums;

namespace ChinaTown.Application.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Content> Content { get; set; }
    public DbSet<Article> Articles { get; set; }
    public DbSet<Book> Books { get; set; }
    public DbSet<Recipe> Recipes { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Like> Likes { get; set; }
    
    public DbSet<ArticleType> ArticleTypes { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<BookGenre> BookGenres { get; set; }
    public DbSet<RecipeType> RecipeTypes { get; set; }
    public DbSet<RecipeTypeClaim> RecipeTypeClaims { get; set; }
    public DbSet<Region> Regions { get; set; }
    public DbSet<RecipeRegion> RecipeRegions { get; set; }
    
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        var contentStatusConverter = new ValueConverter<ContentStatus, string>(
            v => v.ToString(),
            v => (ContentStatus)Enum.Parse(typeof(ContentStatus), v));

        var difficultyConverter = new ValueConverter<RecipeDifficulty, string>(
            v => v.ToString(),
            v => (RecipeDifficulty)Enum.Parse(typeof(RecipeDifficulty), v));

        
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            
            entity.HasIndex(u => u.Email).IsUnique();
            entity.HasIndex(u => u.Username).IsUnique();
            
            entity.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(256);
                
            entity.Property(u => u.Username)
                .IsRequired()
                .HasMaxLength(100);
                
            entity.Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(500);
                
            entity.Property(u => u.CreatedOn)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(u => u.ModifiedOn)
                .HasDefaultValueSql("GETUTCDATE()");
        });
        
        modelBuilder.Entity<Content>(entity =>
        {
            entity.HasIndex(c => c.Slug).IsUnique();
            
            entity.Property(c => c.Title)
                .IsRequired()
                .HasMaxLength(500);
                
            entity.Property(c => c.Slug)
                .IsRequired()
                .HasMaxLength(600);
                
            entity.Property(c => c.Excerpt)
                .HasMaxLength(1000);

            entity.Property(c => c.Status)
                .HasConversion(contentStatusConverter)
                .HasMaxLength(50);
                
            entity.Property(c => c.CreatedOn)
                .HasDefaultValueSql("GETUTCDATE()");
                
            entity.Property(c => c.ModifiedOn)
                .HasDefaultValueSql("GETUTCDATE()");
                
            entity.HasOne(c => c.Author)
                .WithMany(u => u.Contents)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        
        modelBuilder.Entity<Article>(entity =>
        {
            entity.ToTable("Articles");
            
            entity.HasBaseType<Content>();
            
            entity.Property(a => a.Body)
                .IsRequired();
                
            entity.HasOne(a => a.ArticleType)
                .WithMany(at => at.Articles)
                .HasForeignKey(a => a.ArticleTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ArticleType>(entity =>
        {
            entity.ToTable("ArticleTypes");
            
            entity.HasIndex(at => at.Name).IsUnique();
            entity.Property(at => at.Name)
                .IsRequired()
                .HasMaxLength(200);
                
            entity.Property(at => at.CreatedOn)
                .HasDefaultValueSql("GETUTCDATE()");
                
            entity.Property(at => at.ModifiedOn)
                .HasDefaultValueSql("GETUTCDATE()");
        });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.ToTable("Books");
            
            entity.HasBaseType<Content>();
            
            entity.Property(b => b.AuthorName)
                .IsRequired()
                .HasMaxLength(200);
                
            entity.Property(b => b.CoverFileId)
                .IsRequired()
                .HasMaxLength(500);
                
            entity.Property(b => b.Description)
                .IsRequired()
                .HasMaxLength(2000);
        });

        modelBuilder.Entity<Genre>(entity =>
        {
            entity.ToTable("Genres");
            
            entity.HasIndex(g => g.Name).IsUnique();
            entity.Property(g => g.Name)
                .IsRequired()
                .HasMaxLength(100);
                
            entity.Property(g => g.CreatedOn)
                .HasDefaultValueSql("GETUTCDATE()");
                
            entity.Property(g => g.ModifiedOn)
                .HasDefaultValueSql("GETUTCDATE()");
        });

        modelBuilder.Entity<BookGenre>(entity =>
        {
            entity.ToTable("BookGenres");
            
            entity.HasKey(bg => new { bg.BookId, bg.GenreId });
            
            entity.HasOne(bg => bg.Book)
                .WithMany(b => b.BookGenres)
                .HasForeignKey(bg => bg.BookId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(bg => bg.Genre)
                .WithMany(g => g.BookGenres)
                .HasForeignKey(bg => bg.GenreId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Recipe>(entity =>
        {
            entity.ToTable("Recipes");
            
            entity.HasBaseType<Content>();

            entity.Property(r => r.ImageId)
                .IsRequired();
            
            entity.Property(r => r.Difficulty)
                .HasConversion(difficultyConverter)
                .HasMaxLength(50);
                
            entity.Property(r => r.Ingredients)
                .IsRequired();
                
            entity.Property(r => r.Instructions)
                .IsRequired();
        });

        modelBuilder.Entity<RecipeType>(entity =>
        {
            entity.ToTable("RecipeTypes");
            
            entity.HasIndex(rt => rt.Name).IsUnique();
            entity.Property(rt => rt.Name)
                .IsRequired()
                .HasMaxLength(200);
                
            entity.Property(rt => rt.CreatedOn)
                .HasDefaultValueSql("GETUTCDATE()");
                
            entity.Property(rt => rt.ModifiedOn)
                .HasDefaultValueSql("GETUTCDATE()");
        });

        modelBuilder.Entity<RecipeTypeClaim>(entity =>
        {
            entity.ToTable("RecipeTypeClaims");
            
            entity.HasKey(rtc => new { rtc.RecipeId, rtc.RecipeTypeId });
            
            entity.HasOne(rtc => rtc.Recipe)
                .WithMany(r => r.RecipeTypeClaims)
                .HasForeignKey(rtc => rtc.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(rtc => rtc.RecipeType)
                .WithMany(rt => rt.RecipeTypeClaims)
                .HasForeignKey(rtc => rtc.RecipeTypeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Region>(entity =>
        {
            entity.ToTable("Regions");
            
            entity.HasIndex(r => r.Name).IsUnique();
            entity.Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(200);
                
            entity.Property(r => r.CreatedOn)
                .HasDefaultValueSql("GETUTCDATE()");
                
            entity.Property(r => r.ModifiedOn)
                .HasDefaultValueSql("GETUTCDATE()");
        });

        modelBuilder.Entity<RecipeRegion>(entity =>
        {
            entity.ToTable("RecipeRegions");
            
            entity.HasKey(rr => new { rr.RecipeId, rr.RegionId });
            
            entity.HasOne(rr => rr.Recipe)
                .WithMany(r => r.RecipeRegions)
                .HasForeignKey(rr => rr.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(rr => rr.Region)
                .WithMany(r => r.RecipeRegions)
                .HasForeignKey(rr => rr.RegionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.ToTable("Comments");
            
            entity.Property(c => c.Text)
                .IsRequired()
                .HasMaxLength(2000);
                
            entity.Property(c => c.CreatedOn)
                .HasDefaultValueSql("GETUTCDATE()");
                
            entity.Property(c => c.ModifiedOn)
                .HasDefaultValueSql("GETUTCDATE()");
                
            entity.HasOne(c => c.Content)
                .WithMany(c => c.Comments)
                .HasForeignKey(c => c.ContentId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Like>(entity =>
        {
            entity.ToTable("Likes");
            
            entity.HasIndex(l => new { l.ContentId, l.UserId }).IsUnique();
            
            entity.Property(l => l.CreatedOn)
                .HasDefaultValueSql("GETUTCDATE()");
                
            entity.Property(l => l.ModifiedOn)
                .HasDefaultValueSql("GETUTCDATE()");
            
            entity.HasOne(l => l.Content)
                .WithMany(c => c.Likes)
                .HasForeignKey(l => l.ContentId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(l => l.User)
                .WithMany(u => u.Likes)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.ToTable("RefreshTokens");
            
            entity.HasIndex(rt => rt.Token).IsUnique();
            entity.HasIndex(rt => rt.UserId);
            entity.HasIndex(rt => rt.Expires);
            
            entity.Property(rt => rt.Token)
                .IsRequired()
                .HasMaxLength(500);
                
            entity.Property(rt => rt.CreatedOn)
                .HasDefaultValueSql("GETUTCDATE()");
                
            entity.Property(rt => rt.ModifiedOn)
                .HasDefaultValueSql("GETUTCDATE()");
                
            entity.HasOne(rt => rt.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is BaseEntity && 
                       (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entityEntry in entries)
        {
            var entity = (BaseEntity)entityEntry.Entity;
            
            if (entityEntry.State == EntityState.Added)
            {
                entity.CreatedOn = DateTime.UtcNow;
            }
            
            entity.ModifiedOn = DateTime.UtcNow;
        }
    }
}