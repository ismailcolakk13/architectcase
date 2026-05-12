using DigitalLoanSystem.Application.Services;
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
    {
        return await _customerRepository.GetAllAsync();
    }

    public async Task<Customer?> GetCustomerByIdAsync(int id)
    {
        return await _customerRepository.GetByIdAsync(id);
    }

    public async Task<Customer> CreateCustomerAsync(Customer customer)
    {
        await _customerRepository.AddAsync(customer);
        await _customerRepository.SaveChangesAsync();
        return customer;
    }

    public async Task UpdateCustomerAsync(Customer customer)
    {
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