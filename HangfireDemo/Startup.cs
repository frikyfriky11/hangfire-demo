using Hangfire;
using Hangfire.SqlServer;
using HangfireDemo.Jobs;
using HangfireDemo.Services;

namespace HangfireDemo;

public class Startup
{
  private readonly IConfiguration _configuration;

  public Startup(IConfiguration configuration)
  {
    _configuration = configuration;
  }
  
  public void ConfigureServices(IServiceCollection services)
  {
    services.AddControllers();

    services.AddSingleton<IRepository, InMemoryRepository>();

    services.AddHangfire(configuration => configuration
      .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
      .UseSimpleAssemblyNameTypeSerializer()
      .UseRecommendedSerializerSettings()
      .UseSqlServerStorage(_configuration.GetConnectionString("HangfireDb"), new SqlServerStorageOptions()
      {
        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
        QueuePollInterval = TimeSpan.Zero,
        UseRecommendedIsolationLevel = true,
        DisableGlobalLocks = true,
      }));

    services.AddHangfireServer();
  }

  public void Configure(IApplicationBuilder app)
  {
    app.UseRouting();

    app.UseEndpoints(endpoints =>
    {
      endpoints.MapControllers();
      endpoints.MapHangfireDashboard();
    });

    RecurringJob.AddOrUpdate<RecurringCustomersJobs>("daily-report", job => job.HourlyReport(), Cron.Daily(7, 0));
  }
}
