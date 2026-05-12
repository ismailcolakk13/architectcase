using DigitalLoanSystem.Application.DTOs;
using DigitalLoanSystem.Core.Entities;

namespace DigitalLoanSystem.Application.Services;

public interface ILoanService
{
    Task<Loan> CreateLoanWithInstallmentsAsync(CreateLoanDto dto);
}