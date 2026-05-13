using DigitalLoanSystem.Application.DTOs;
using DigitalLoanSystem.Core.Entities;
using DigitalLoanSystem.Core.Interfaces;

namespace DigitalLoanSystem.Application.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerService(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
        => await _customerRepository.GetAllAsync();

    public async Task<Customer?> GetCustomerByIdAsync(int id)
        => await _customerRepository.GetByIdAsync(id);

    public async Task<Customer?> GetCustomerByIdentityNumberAsync(string identityNumber)
        => await _customerRepository.GetByIdentityNumberAsync(identityNumber);

    public async Task<Customer> CreateCustomerAsync(CreateCustomerDto dto)
    {
        var customer = new Customer
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Phone = dto.Phone,
            Address = dto.Address,
            IdentityNumber = dto.IdentityNumber,
            CreditScore = dto.CreditScore > 0 ? dto.CreditScore : 1200
        };
        await _customerRepository.AddAsync(customer);
        await _customerRepository.SaveChangesAsync();
        return customer;
    }

    public async Task UpdateCustomerAsync(int id, UpdateCustomerDto dto)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        if (customer == null)
            throw new ArgumentException("Müşteri bulunamadı.");

        customer.FirstName = dto.FirstName;
        customer.LastName = dto.LastName;
        customer.Email = dto.Email;
        customer.Phone = dto.Phone;
        customer.Address = dto.Address;
        if (dto.CreditScore > 0)
            customer.CreditScore = dto.CreditScore;

        _customerRepository.Update(customer);
        await _customerRepository.SaveChangesAsync();
    }

    public async Task DeleteCustomerAsync(int id)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        if (customer != null)
        {
            _customerRepository.Delete(customer);
            await _customerRepository.SaveChangesAsync();
        }
    }
}