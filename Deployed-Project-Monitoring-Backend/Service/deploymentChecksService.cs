using Data;
using DeploymentChecksInterface;
using Enums;
using Microsoft.EntityFrameworkCore;
using Models;
using System.Diagnostics;

namespace Services;

public class DeploymentChecksService : IDeploymentChecksService
{
    private readonly ApplicationDbContext _context;
    private readonly HttpClient _httpClient;

    public DeploymentChecksService(
        ApplicationDbContext context,
        HttpClient httpClient)
    {
        _context = context;
        _httpClient = httpClient;
    }

    public async Task<DeploymentChecks?> CheckProject(int projectId)
    {
        var project = await _context.Projects
            .FirstOrDefaultAsync(p => p.id == projectId);

        if (project == null)
            return null;

        var check = new DeploymentChecks
        {
            project_id = project.id,
            checked_at = DateTime.UtcNow
        };

        var stopwatch = Stopwatch.StartNew();

        try
        {
            using var response = await _httpClient.GetAsync(
                project.deployment_url,
                HttpCompletionOption.ResponseHeadersRead
            );

            stopwatch.Stop();

            check.http_status_code = (int)response.StatusCode;
            check.response_time_ms = stopwatch.ElapsedMilliseconds;

            check.status = response.IsSuccessStatusCode
                ? "Online"
                : "Offline";

            check.error_message = null;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            check.response_time_ms = stopwatch.ElapsedMilliseconds;
            check.status = "Offline";
            check.error_message = ex.Message;
        }

        // Update the project's latest monitoring information
        project.last_checked_at = check.checked_at;

        project.status = check.status == "Online"
            ? DeploymentStatus.Online
            : DeploymentStatus.Offline;

        // Save the monitoring history
        _context.DeploymentChecks.Add(check);

        await _context.SaveChangesAsync();

        return check;
    }
}