using DigitalLoanSystem.Application.DTOs;
using DigitalLoanSystem.Application.Services;
using DigitalLoanSystem.Core.Entities;
using DigitalLoanSystem.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Loan>>> GetLoans()
    {
        var loans = await _loanService.GetAllLoansAsync();
        return Ok(loans);
    }

    // GET: api/loans/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Loan>> GetLoan(int id)
    {
        var loan = await _loanService.GetLoanByIdAsync(id);

        if (loan == null)
        {
            return NotFound(new { Message = "Kredi bulunamadı." });
        }

        return loan;
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
            return StatusCode(500, new { Message = "Kredi oluşturulurken sistemsel bir hata meydana geldi.", Details = ex.Message });
        }

    }

    // PUT: api/loans/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateLoan(int id, Loan loan)
    {
        if (id != loan.Id) return BadRequest(new { Message = "ID uyuşmazlığı" });

        var existingLoan = await _loanService.GetLoanByIdAsync(id);
        if (existingLoan == null) return NotFound(new { Message = "Kredi bulunamadı." });

        await _loanService.UpdateLoanAsync(loan);
        return Ok(new { Message = $"Kredi {id} güncellendi." });
    }
}