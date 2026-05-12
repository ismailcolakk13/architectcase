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
    private readonly AppDbContext _context;

    public LoansController(ILoanService loanService, AppDbContext context)
    {
        _loanService = loanService;
        _context = context;
    }

    // GET: api/loans
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Loan>>> GetLoans()
    {
        return await _context.Loans
            .Include(l => l.Installments)
            .ToListAsync();
    }

    // GET: api/loans/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Loan>> GetLoan(int id)
    {
        var loan = await _context.Loans
            .Include(l => l.Installments)
            .FirstOrDefaultAsync(l => l.Id == id);

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
        var customerExists = await _context.Customers.AnyAsync(c => c.Id == dto.CustomerId);
        if (!customerExists)
        {
            return BadRequest(new { Message = "Belirtilen ID'ye sahip müşteri yok!" });
        }

        var loan = await _loanService.CreateLoanWithInstallmentsAsync(dto);

        return CreatedAtAction(nameof(GetLoan), new { id = loan.Id }, loan);
    }

    // PUT: api/loans/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateLoan(int id, Loan loan)
    {
        if (id != loan.Id)
        {
            return BadRequest(new { Message = "ID uyuşmazlığı" });
        }

        _context.Entry(loan).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Loans.Any(e => e.Id == id))
            {
                return NotFound(new { Message = "Kredi bulunamadı." });
            }
            throw;
        }

        return Ok(new { Message = $"Kredi {id} güncellendi." });
    }
}