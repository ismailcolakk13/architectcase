using DigitalLoanSystem.Core.Entities;
using DigitalLoanSystem.Core.Interfaces;
using DigitalLoanSystem.Infrastructure.Data;

namespace DigitalLoanSystem.Infrastructure.Repositories;

public class LoanRepository : ILoanRepository
{
    private readonly AppDbContext _context;

    public LoanRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Loan> AddAsync(Loan loan)
    {
        await _context.Loans.AddAsync(loan);
        return loan;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}