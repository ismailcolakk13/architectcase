using DigitalLoanSystem.Application.DTOs;

namespace DigitalLoanSystem.Application.Services;

public interface ICustomerSummaryService
{
    Task<CustomerSummaryDto?> GetCustomerSummaryAsync(int customerId);
}