using HangfireDemo.Services;

namespace HangfireDemo;

public class Startup
{
  public void ConfigureServices(IServiceCollection services)
  {
    services.AddControllers();

    services.AddSingleton<IRepository, InMemoryRepository>();
  }

  public void Configure(IApplicationBuilder app)
  {
    app.UseRouting();

    app.UseEndpoints(endpoints => endpoints.MapControllers());
  }
}
