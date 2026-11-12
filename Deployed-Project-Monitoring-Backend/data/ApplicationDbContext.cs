using Microsoft.EntityFrameworkCore;
using Models;

namespace Data;

public class ApplicationDbContext : DbContext
{
	public ApplicationDbContext(
		DbContextOptions<ApplicationDbContext> options)
		: base(options)
	{
	}

	public DbSet<Users> Users { get; set; }
    public DbSet<Projects> Projects { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
		modelBuilder.Entity<Projects>()
			.HasOne(p => p.User)
			.WithMany(u => u.Projects)
			.HasForeignKey(p => p.user_id)
			.OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Projects>()
    .Property(p => p.status)
    .HasConversion<string>();
    }
}