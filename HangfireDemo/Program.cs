namespace HangfireDemo;

public static class Program
{
  public static int Main(string[] args)
  {
    CreateHostBuilder(args).Build().Run();

    return 0;
  }

  private static IHostBuilder CreateHostBuilder(string[] args)
  {
    return Host.CreateDefaultBuilder(args)
      .ConfigureWebHostDefaults(builder => builder.UseStartup<Startup>());
  }
}