using DigitalLoanSystem.Core.Entities;
using DigitalLoanSystem.Core.Interfaces;
using DigitalLoanSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DigitalLoanSystem.Infrastructure.Repositories;

public class InstallmentRepository : IInstallmentRepository
{
    private readonly AppDbContext _context;

    public InstallmentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Installment?> GetByIdAsync(int id)
        => await _context.Installments
            .Include(i => i.Loan)
            .Include(i => i.Payment)
            .FirstOrDefaultAsync(i => i.Id == id);

    public async Task<IEnumerable<Installment>> GetByLoanIdAsync(int loanId)
        => await _context.Installments
            .Include(i => i.Payment)
            .Where(i => i.LoanId == loanId)
            .OrderBy(i => i.InstallmentNumber)
            .ToListAsync();

    public async Task<IEnumerable<Installment>> GetByCustomerIdAsync(int customerId)
        => await _context.Installments
            .Include(i => i.Loan)
            .Include(i => i.Payment)
            .Where(i => i.Loan.CustomerId == customerId)
            .OrderBy(i => i.LoanId)
            .ThenBy(i => i.InstallmentNumber)
            .ToListAsync();

    public async Task SaveChangesAsync()
        => await _context.SaveChangesAsync();
}
