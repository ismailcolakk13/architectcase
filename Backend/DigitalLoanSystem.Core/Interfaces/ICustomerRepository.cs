using DigitalLoanSystem.Core.Entities;

namespace DigitalLoanSystem.Core.Interfaces;

public interface ICustomerRepository
{
    Task<IEnumerable<Customer>> GetAllAsync();
    Task<Customer?> GetByIdAsync(int id);
    Task<Customer?> GetCustomerWithLoansAndInstallmentsAsync(int customerId);
    Task AddAsync(Customer customer);
    void Update(Customer customer);
    void Delete(Customer customer);
    Task SaveChangesAsync();
}