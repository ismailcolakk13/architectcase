using DigitalLoanSystem.Core.Entities;

namespace DigitalLoanSystem.Application.Services;

public interface IInstallmentService
{
    Task<Installment?> GetInstallmentByIdAsync(int id);
    Task<IEnumerable<Installment>> GetInstallmentsByLoanIdAsync(int loanId);
    Task<IEnumerable<Installment>> GetInstallmentsByCustomerIdAsync(int customerId);
}
