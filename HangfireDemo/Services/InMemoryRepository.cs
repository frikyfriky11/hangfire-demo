using HangfireDemo.Models;

namespace HangfireDemo.Services;

public class InMemoryRepository : IRepository
{
  private readonly List<Customer> _customers;

  public InMemoryRepository()
  {
    _customers = new List<Customer>
    {
      new(1, "John"),
      new(2, "Paul"),
    };
  }

  public List<Customer> GetList()
  {
    return _customers;
  }

  public Customer? Get(int id)
  {
    return _customers.Find(c => c.Id == id);
  }

  public void Add(Customer customer)
  {
    _customers.Add(customer);
  }

  public void Update(int id, Customer customer)
  {
    Customer? existing = _customers.Find(c => c.Id == id);

    if (existing != null)
    {
      existing.Name = customer.Name;
    }
  }

  public void Delete(int id)
  {
    _customers.RemoveAll(c => c.Id == id);
  }
}
