using DigitalLoanSystem.Application.DTOs;
using DigitalLoanSystem.Application.Services;
using DigitalLoanSystem.Core.Entities;
using Microsoft.AspNetCore.Mvc;

namespace DigitalLoanSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoansController : ControllerBase
{
    private readonly ILoanService _loanService;

    public LoansController(ILoanService loanService)
    {
        _loanService = loanService;
    }

    // GET: api/loans
    // GET: api/loans?customerId=5
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Loan>>> GetLoans([FromQuery] int? customerId)
    {
        if (customerId.HasValue)
        {
            var customerLoans = await _loanService.GetLoansByCustomerIdAsync(customerId.Value);
            return Ok(customerLoans);
        }

        var loans = await _loanService.GetAllLoansAsync();
        return Ok(loans);
    }

    // GET: api/loans/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Loan>> GetLoan(int id)
    {
        var loan = await _loanService.GetLoanByIdAsync(id);
        if (loan == null)
            return NotFound(new { Message = "Kredi bulunamadı." });

        return Ok(loan);
    }

    // POST: api/loans
    [HttpPost]
    public async Task<ActionResult<Loan>> CreateLoan(CreateLoanDto dto)
    {
        try
        {
            var loan = await _loanService.CreateLoanWithInstallmentsAsync(dto);
            return CreatedAtAction(nameof(GetLoan), new { id = loan.Id }, loan);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Kredi oluşturulurken bir hata meydana geldi.", Details = ex.Message });
        }
    }

    // PUT: api/loans/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateLoan(int id, UpdateLoanDto dto)
    {
        try
        {
            await _loanService.UpdateLoanAsync(id, dto);
            return Ok(new { Message = $"Kredi {id} güncellendi." });
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
    }
}