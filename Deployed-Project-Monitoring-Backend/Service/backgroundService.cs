using DeploymentChecksInterface;

using ProjectInterface;

namespace BackgroundServices;

public class DeploymentMonitoringWorker : BackgroundService
{
	private readonly IServiceScopeFactory _scopeFactory;
	private readonly ILogger<DeploymentMonitoringWorker> _logger;

	public DeploymentMonitoringWorker(
		IServiceScopeFactory scopeFactory,
		ILogger<DeploymentMonitoringWorker> logger)
	{
		_scopeFactory = scopeFactory;
		_logger = logger;
	}

	protected override async Task ExecuteAsync(
		CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			try
			{
				using var scope = _scopeFactory.CreateScope();

				var monitor = scope.ServiceProvider
					.GetRequiredService<IDeploymentChecksService>();

				await monitor.CheckAllProjects();

				_logger.LogInformation(
					"Automatic deployment monitoring completed.");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex,
					"Error during automatic deployment monitoring.");
			}

			await Task.Delay(
				TimeSpan.FromMinutes(5),
				stoppingToken);
		}
	}
}