using DigitalLoanSystem.Core.Entities;
using DigitalLoanSystem.Core.Interfaces;
using DigitalLoanSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

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

    public async Task<IEnumerable<Loan>> GetAllAsync()
        => await _context.Loans.Include(l => l.Customer).Include(l => l.Installments).ToListAsync();

    public async Task<Loan?> GetByIdWithInstallmentsAsync(int id)
        => await _context.Loans.Include(l => l.Customer).Include(l => l.Installments).FirstOrDefaultAsync(l => l.Id == id);

    public async Task<IEnumerable<Loan>> GetByCustomerIdAsync(int customerId)
        => await _context.Loans
            .Include(l => l.Customer)
            .Include(l => l.Installments)
            .Where(l => l.CustomerId == customerId)
            .ToListAsync();

    public void Update(Loan loan)
        => _context.Loans.Update(loan);
}