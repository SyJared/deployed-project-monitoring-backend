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
	public DbSet<DeploymentChecks> DeploymentChecks { get; set; }
    public DbSet<DowntimeEvents> DowntimeEvents { get; set; }
    public DbSet<DeploymentReports> DeploymentReports { get; set; }
    public DbSet<Heartbeats> Heartbeats { get; set; }

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
        modelBuilder.Entity<DowntimeEvents>()
		.Property(d => d.status)
		.HasConversion<string>();
        modelBuilder.Entity<DowntimeEvents>()	
		.HasOne(d => d.Project)
		.WithMany()
		.HasForeignKey(d => d.project_id)
		.OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<DeploymentChecks>()
		.HasOne(d => d.Project)
		.WithMany(p => p.DeploymentChecks)
		.HasForeignKey(d => d.project_id)
		.OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<DeploymentReports>()
		.HasOne(d => d.Project)
		.WithMany()
		.HasForeignKey(d => d.project_id)
		.OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Projects>()
		.Property(p => p.api_key_hash)
		.HasMaxLength(64);

        modelBuilder.Entity<Projects>()
        .HasIndex(p => p.api_key_hash)
        .IsUnique();

        modelBuilder.Entity<Heartbeats>(e =>
        {
            e.Property(h => h.instance_id).HasMaxLength(64);
            e.HasIndex(h => new { h.project_id, h.instance_id }).IsUnique();
        });
    }

}