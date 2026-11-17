
using Data;
using DTOs;
using Microsoft.EntityFrameworkCore;
using Models;
using SDKInterface;

namespace SDKServices;

public class DeploymentReportService : IDeploymentReportService
{
    private readonly ApplicationDbContext _context;

    public DeploymentReportService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DeploymentReports?> ReportDeployment(
        DeploymentReportRequest request)
    {
        var project = await _context.Projects
            .FirstOrDefaultAsync(p => p.id == request.project_id);

        if (project == null)
            return null;

        var report = new DeploymentReports
        {
            project_id = project.id,
            version = request.version,
            environment = request.environment,
            deployment_id = request.deployment_id,
            commit_hash = request.commit_hash,
            deployed_at = request.deployed_at ?? DateTime.UtcNow,
            received_at = DateTime.UtcNow
        };

        _context.DeploymentReports.Add(report);

        await _context.SaveChangesAsync();

        return report;
    }
    public async Task<List<DeploymentReports>> GetDeploymentHistory(
    int projectId)
    {
        return await _context.DeploymentReports
            .AsNoTracking()
            .Where(d => d.project_id == projectId)
            .OrderByDescending(d => d.deployed_at)
            .ToListAsync();
    }
}
