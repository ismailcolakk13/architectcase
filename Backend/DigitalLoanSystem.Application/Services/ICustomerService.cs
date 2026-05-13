using DigitalLoanSystem.Application.DTOs;
using DigitalLoanSystem.Core.Entities;

namespace DigitalLoanSystem.Application.Services;

public interface ICustomerService
{
    Task<IEnumerable<Customer>> GetAllCustomersAsync();
    Task<Customer?> GetCustomerByIdAsync(int id);
    Task<Customer?> GetCustomerByIdentityNumberAsync(string identityNumber);
    Task<Customer> CreateCustomerAsync(CreateCustomerDto dto);
    Task UpdateCustomerAsync(int id, UpdateCustomerDto dto);
    Task DeleteCustomerAsync(int id);
}