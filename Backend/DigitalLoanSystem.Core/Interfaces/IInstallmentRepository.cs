using DigitalLoanSystem.Core.Entities;

namespace DigitalLoanSystem.Core.Interfaces;

public interface IInstallmentRepository
{
    Task<Installment?> GetByIdAsync(int id);
    Task<IEnumerable<Installment>> GetByLoanIdAsync(int loanId);
    Task<IEnumerable<Installment>> GetByCustomerIdAsync(int customerId);
    Task SaveChangesAsync();
}
