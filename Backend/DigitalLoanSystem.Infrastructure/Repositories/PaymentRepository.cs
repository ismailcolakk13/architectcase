using DigitalLoanSystem.Core.Entities;
using DigitalLoanSystem.Core.Interfaces;
using DigitalLoanSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DigitalLoanSystem.Infrastructure.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly AppDbContext _context;

    public PaymentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Installment?> GetInstallmentByIdAsync(int installmentId)
    {
        return await _context.Installments
            .Include(i => i.Loan)
                .ThenInclude(l => l.Customer)
            .FirstOrDefaultAsync(i => i.Id == installmentId);
    }

    public async Task<Payment> SavePaymentAndUpdateInstallmentAsync(Payment payment, Installment installment)
    {
        _context.Installments.Update(installment);
        await _context.Payments.AddAsync(payment);
        await _context.SaveChangesAsync();

        return payment;
    }

    public async Task<IEnumerable<Payment>> GetAllAsync() => await _context.Payments.ToListAsync();

    public async Task<Payment?> GetByIdAsync(int id)
        => await _context.Payments
            .Include(p => p.Installment)
            .FirstOrDefaultAsync(p => p.Id == id);

    public async Task<Payment?> GetByInstallmentIdAsync(int installmentId)
        => await _context.Payments
            .FirstOrDefaultAsync(p => p.InstallmentId == installmentId);
}