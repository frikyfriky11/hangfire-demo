using HangfireDemo.Models;
using HangfireDemo.Services;

namespace HangfireDemo.Jobs;

public class CustomersJobs
{
  private readonly ILogger<CustomersJobs> _logger;
  private readonly IRepository _repository;

  public CustomersJobs(IRepository repository, ILogger<CustomersJobs> logger)
  {
    _repository = repository;
    _logger = logger;
  }

  public void SendGoodbyeMail(int customerId)
  {
    Customer? customer = _repository.Get(customerId);
    _logger.LogInformation("Sending goodbye email to customer {CustomerId} with name {CustomerName}", customerId, customer?.Name);
  }

  public void SendUpdateMail(int customerId, string? customerName)
  {
    _logger.LogInformation("Sending update email to customer {CustomerId} with name {CustomerName}", customerId, customerName);
  }
}
