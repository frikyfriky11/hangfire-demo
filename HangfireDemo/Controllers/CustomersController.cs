using HangfireDemo.Models;
using HangfireDemo.Services;
using Microsoft.AspNetCore.Mvc;

namespace HangfireDemo.Controllers;

public class CustomersController : ControllerBase
{
  [HttpGet]
  [Route("customers")]
  public ActionResult<IEnumerable<Customer>> GetList([FromServices] IRepository repository)
  {
    return repository.GetList();
  }

  [HttpGet]
  [Route("customers/{id}")]
  public ActionResult<Customer> GetById([FromServices] IRepository repository, int id)
  {
    return repository.Get(id) ?? (ActionResult<Customer>)NotFound();
  }

  [HttpPost]
  [Route("customers")]
  public ActionResult Create([FromServices] IRepository repository, [FromBody] Customer customer)
  {
    repository.Add(customer);

    return NoContent();
  }

  [HttpPut]
  [Route("customers/{id}")]
  public ActionResult Update([FromServices] IRepository repository, int id, [FromBody] Customer customer)
  {
    repository.Update(id, customer);

    return NoContent();
  }

  [HttpDelete]
  [Route("customers/{id}")]
  public ActionResult Delete([FromServices] IRepository repository, int id)
  {
    repository.Delete(id);

    return NoContent();
  }
}
