using BackgroundServices;
using Data;
using DeploymentChecksInterface;
using Microsoft.EntityFrameworkCore;
using ProjectInterface;
using SDKInterface;
using SDKServices;
using Services;
using UserInterfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddHttpClient<
    IDeploymentChecksService,
    DeploymentChecksService>(client =>
    {
        client.Timeout = TimeSpan.FromSeconds(15);
    });
builder.Services.AddHostedService<DeploymentMonitoringWorker>();


builder.Services.AddScoped<
    IDeploymentReportService,
    DeploymentReportService>();

builder.Services.AddScoped<IHeartbeatService, HeartbeatService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
