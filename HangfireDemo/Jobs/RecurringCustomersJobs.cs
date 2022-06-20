using HangfireDemo.Models;
using HangfireDemo.Services;

namespace HangfireDemo.Jobs;

public class RecurringCustomersJobs
{
  private readonly IRepository _repository;

  public RecurringCustomersJobs(IRepository repository)
  {
    _repository = repository;
  }

  public void HourlyReport()
  {
    List<Customer> customers = _repository.GetList();

    for (int i = 0; i < customers.Count; i++)
    {
      Customer customer = customers[i];
      Console.WriteLine($"[{i}] - {customer.Id} - {customer.Name}");
    }
  }
}
