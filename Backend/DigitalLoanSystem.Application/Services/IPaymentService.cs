using DigitalLoanSystem.Application.DTOs;
using DigitalLoanSystem.Core.Entities;

namespace DigitalLoanSystem.Application.Services;

public interface IPaymentService
{
    Task<Payment> CreatePaymentAsync(CreatePaymentDto dto);
    Task<IEnumerable<Payment>> GetAllPaymentsAsync();
    Task<Payment?> GetPaymentByIdAsync(int id);
    Task<Payment?> GetPaymentByInstallmentIdAsync(int installmentId);
}