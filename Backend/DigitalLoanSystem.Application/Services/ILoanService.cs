using DigitalLoanSystem.Application.DTOs;
using DigitalLoanSystem.Core.Entities;

namespace DigitalLoanSystem.Application.Services;

public interface ILoanService
{
    Task<Loan> CreateLoanWithInstallmentsAsync(CreateLoanDto dto);
    Task<IEnumerable<Loan>> GetAllLoansAsync();
    Task<Loan?> GetLoanByIdAsync(int id);
    Task UpdateLoanAsync(Loan loan);
}