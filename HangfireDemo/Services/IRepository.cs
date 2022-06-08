using HangfireDemo.Models;

namespace HangfireDemo.Services;

public interface IRepository
{
  List<Customer> GetList();

  Customer? Get(int id);

  void Add(Customer customer);

  void Update(int id, Customer customer);

  void Delete(int id);
}
