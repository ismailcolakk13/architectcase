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
            return NotFound(new { Message = "Müşteri bulunamadı." });

        return Ok(customer);
    }

    // GET: api/customers/{id}/summary
    [HttpGet("{id}/summary")]
    public async Task<ActionResult<CustomerSummaryDto>> GetCustomerSummary(int id)
    {
        var summary = await _summaryService.GetCustomerSummaryAsync(id);
        if (summary == null)
            return NotFound(new { Message = "Müşteri bulunamadı." });

        return Ok(summary);
    }

    // GET: api/customers/by-identity/{identityNumber}
    [HttpGet("by-identity/{identityNumber}")]
    public async Task<ActionResult<Customer>> GetCustomerByIdentityNumber(string identityNumber)
    {
        var customer = await _customerService.GetCustomerByIdentityNumberAsync(identityNumber);
        if (customer == null)
            return NotFound(new { Message = "Belirtilen T.C. Kimlik numarasına sahip müşteri bulunamadı." });

        return Ok(customer);
    }

    // POST: api/customers
    [HttpPost]
    public async Task<ActionResult<Customer>> CreateCustomer(CreateCustomerDto dto)
    {
        var created = await _customerService.CreateCustomerAsync(dto);
        return CreatedAtAction(nameof(GetCustomer), new { id = created.Id }, created);
    }

    // PUT: api/customers/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCustomer(int id, UpdateCustomerDto dto)
    {
        try
        {
            await _customerService.UpdateCustomerAsync(id, dto);
            return Ok(new { Message = $"Müşteri {id} güncellendi." });
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
    }

    // DELETE: api/customers/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCustomer(int id)
    {
        await _customerService.DeleteCustomerAsync(id);
        return Ok(new { Message = $"Müşteri {id} silindi." });
    }
}