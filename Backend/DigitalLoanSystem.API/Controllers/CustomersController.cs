using DigitalLoanSystem.Application.DTOs;
using DigitalLoanSystem.Application.Services;
using DigitalLoanSystem.Core.Entities;
using Microsoft.AspNetCore.Mvc;

namespace DigitalLoanSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;
    private readonly ICustomerSummaryService _summaryService;

    public CustomersController(ICustomerService customerService, ICustomerSummaryService summaryService)
    {
        _customerService = customerService;
        _summaryService = summaryService;
    }

    // GET: api/customers
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
    {
        var customers = await _customerService.GetAllCustomersAsync();
        return Ok(customers);
    }

    // GET: api/customers/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Customer>> GetCustomer(int id)
    {
        var customer = await _customerService.GetCustomerByIdAsync(id);
        if (customer == null)
        {
            return NotFound(new { message = "Müşteri bulunamadı." });
        }
        return customer;
    }

    // GET: api/customers/{id}/summary
    [HttpGet("{id}/summary")]
    public async Task<ActionResult<CustomerSummaryDto>> GetCustomerSummary(int id)
    {
        var summary = await _summaryService.GetCustomerSummaryAsync(id);
        if (summary == null) return NotFound(new { message = "Müşteri bulunamadı." });
        return Ok(summary);
    }

    // POST: api/customers
    [HttpPost]
    public async Task<ActionResult<Customer>> CreateCustomer(Customer customer)
    {
        var created = await _customerService.CreateCustomerAsync(customer);
        return CreatedAtAction(nameof(GetCustomer), new { id = created.Id }, created);
    }

    // PUT: api/customers/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCustomer(int id, Customer customer)
    {
        if (id != customer.Id)
        {
            return BadRequest(new { Message = "ID uyuşmazlığı." });
        }
        await _customerService.UpdateCustomerAsync(customer);
        return Ok(new { Message = $"Müşteri {id} güncellendi" });
    }

    // DELETE: api/customers/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCustomer(int id)
    {
        await _customerService.DeleteCustomerAsync(id);
        return Ok(new { Message = $"Müşteri {id} silindi" });
    }
}