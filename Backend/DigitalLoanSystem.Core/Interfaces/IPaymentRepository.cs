using DigitalLoanSystem.Core.Entities;

namespace DigitalLoanSystem.Core.Interfaces;

public interface IPaymentRepository
{
    Task<Installment?> GetInstallmentByIdAsync(int installmentId);
    Task<Payment> SavePaymentAndUpdateInstallmentAsync(Payment payment, Installment installment);
    Task<IEnumerable<Payment>> GetAllAsync();
}