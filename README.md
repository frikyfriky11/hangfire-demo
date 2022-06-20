# Hangfire Demo

This is a demo app to showcase how to use Hangfire for basic stuff such as enqueueing jobs on demand and running recurring jobs in the background.

## Basic structure

The starting point of this app is a sample ASP.NET Core 6 WebAPI project with a single *Customers* controller.

The controller uses an in-memory repository to store and retrieve customers to keep things simple to understand.

If you want, you can [browse the code](https://github.com/frikyfriky11/hangfire-demo/tree/6c6716049e006e98699b9d47efdb541c3687bbb7) before Hangfire is added to see the basic structure of the project.

## Building and running the app

To build the app, run the following commands from a shell:

```shell
# clone the repository on your machine
git clone https://github.com/frikyfriky11/hangfire-demo.git

# change to the directory
cd hangfire-demo\HangFireDemo

# edit the ConnectionString property inside the appsettings.Development.json file to point to your SQL Server instance
notepad appsettings.Development.json

# build and run the app
dotnet run
```

## Setup Hangfire from scratch

To setup Hangfire from scratch, you need to download these dependencies from NuGet:

* Hangfire.Core
* Hangfire.AspNetCore
* Hangfire.SqlServer

Please note that this repository uses version 1.7.29 of the packages.

In the *Startup.cs* file, add the following lines under the `ConfigureServices` method ([link to the code](https://github.com/frikyfriky11/hangfire-demo/blob/dc54889724174f622b0bfb71fe190f83dcb60832/HangfireDemo/Startup.cs#L23-L36)):

```csharp
public void ConfigureServices(IServiceCollection services)
{
  /* ...other stuff... */
  
  services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(_configuration.GetConnectionString("HangfireDb"), new SqlServerStorageOptions
    {
      CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
      SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
      QueuePollInterval = TimeSpan.Zero,
      UseRecommendedIsolationLevel = true,
      DisableGlobalLocks = true,
    }));
  
  services.AddHangfireServer();
  
  /* ...other stuff... */
}
```

and the following lines under the `Configure` method ([link to the code](https://github.com/frikyfriky11/hangfire-demo/blob/dc54889724174f622b0bfb71fe190f83dcb60832/HangfireDemo/Startup.cs#L46)):

```csharp
public void Configure(IApplicationBuilder app)
{
  /* ...other stuff... */
  
  app.UseEndpoints(endpoints =>
  {
    endpoints.MapControllers();
    endpoints.MapHangfireDashboard(); // <- this line
  });
  
  /* ...other stuff... */
}
```

Make sure to add Hangfire to your logging system. This section depends on what logging framework you are using.

If you use the Microsoft built-in logging system, add this to your *appsettings.json* file or environment-specific settings file ([link to the code](https://github.com/frikyfriky11/hangfire-demo/blob/dc54889724174f622b0bfb71fe190f83dcb60832/HangfireDemo/appsettings.Development.json#L6)):

```json
{
  /* ...other stuff... */
  
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Hangfire": "Information" // <- this line
    }
  }
  
  /* ...other stuff... */
}
```

### Creating enqueue-able jobs

Create a new class to hold your enqueue-able jobs ([link to the code](https://github.com/frikyfriky11/hangfire-demo/blob/dc54889724174f622b0bfb71fe190f83dcb60832/HangfireDemo/Jobs/CustomersJobs.cs)), for example:

```csharp
public class CustomersJobs
{
  public void SendGoodbyeMail(int customerId)
  {
    Console.WriteLine($"Sending goodbye email to customer {customerId}");
    
    /* ...your logic here... */
  }

  public void SendUpdateMail(int customerId, string? customerName)
  {
    Console.WriteLine($"Sending update email to customer {customerId} with name {customer.Name}");
    
    /* ...your logic here... */
  }
}
```

Inject the `IBackgroundJobClient` interface into the controller action ([link to the code](https://github.com/frikyfriky11/hangfire-demo/blob/dc54889724174f622b0bfb71fe190f83dcb60832/HangfireDemo/Controllers/CustomersController.cs#L34-L54)):

```csharp
public class CustomersController : ControllerBase
{
  /* ...other stuff... */
  
  [HttpPut]
  [Route("customers/{id}")]
  public ActionResult Update(
    [FromServices] IRepository repository, 
    [FromServices] IBackgroundJobClient jobClient, // <-- this line
    int id, 
    [FromBody] Customer customer)
  {
    jobClient.Enqueue<CustomersJobs>(jobs => jobs.SendUpdateMail(id, customer.Name)); // <-- this line
    
    repository.Update(id, customer);
  
    return NoContent();
  }
  
  /* ...other stuff... */
}
```

### Creating recurring jobs

Create a new class to hold your recurring jobs ([link to the code](https://github.com/frikyfriky11/hangfire-demo/blob/dc54889724174f622b0bfb71fe190f83dcb60832/HangfireDemo/Jobs/RecurringCustomersJobs.cs)), for example:

```csharp
public class RecurringCustomersJobs
{
  public void HourlyReport()
  {
    Console.WriteLine("Creating hourly report");
    
    /* ...your logic here... */
  }
}
```

In the Startup.cs file, add the following lines under the `Configure` method ([link to the code](https://github.com/frikyfriky11/hangfire-demo/blob/dc54889724174f622b0bfb71fe190f83dcb60832/HangfireDemo/Startup.cs#L49)):

```csharp
public void Configure(IApplicationBuilder app)
{
  /* ...other stuff... */
  RecurringJob.AddOrUpdate<RecurringCustomersJobs>("daily-report", job => job.HourlyReport(), Cron.Daily(7, 0)); // <- this line
}
```

Hangfire uses Cron expressions to setup recurring jobs. There is a helper class `Cron` with some useful common expressions ready to use, like for example `Cron.Daily(7, 0)` which means "every day at 7:00 AM".

Remember that the first parameter you supply to the `AddOrUpdate` method (in the example `"daily-report"`) is the name of the job and must be unique for each job definition.

### Injecting dependencies into jobs

Hangfire can inject dependencies into jobs. For example, if you have a job that needs to access a repository, you can inject the repository into the job class like you would normally do in ASP.NET Core with the default Dependency Injection container through constructor injection.

See this example ([link to the code](https://github.com/frikyfriky11/hangfire-demo/blob/dc54889724174f622b0bfb71fe190f83dcb60832/HangfireDemo/Jobs/RecurringCustomersJobs.cs#L8-L13)):

```csharp
public class RecurringCustomersJobs
{
  /* inject services through constructor injection as usual */
  private readonly IRepository _repository;
  
  public RecurringCustomersJobs(IRepository repository)
  {
    _repository = repository;
  }
  
  public void HourlyReport()
  {
    /* use the injected interfaces as usual */
    List<Customer> customers = _repository.GetList();
    
    /* ...your logic here... */  
  }
}
```

This works for recurring jobs as well as enqueue-able jobs.
