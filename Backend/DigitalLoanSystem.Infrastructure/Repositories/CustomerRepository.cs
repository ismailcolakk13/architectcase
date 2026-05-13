using DigitalLoanSystem.Core.Entities;
using DigitalLoanSystem.Core.Interfaces;
using DigitalLoanSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DigitalLoanSystem.Infrastructure.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly AppDbContext _context;

    public CustomerRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Customer>> GetAllAsync()
    {
        return await _context.Customers.ToListAsync();
    }

    public async Task<Customer?> GetByIdAsync(int id)
    {
        return await _context.Customers
            .Include(c => c.Loans)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Customer?> GetByIdentityNumberAsync(string identityNumber)
    {
        return await _context.Customers
            .Include(c => c.Loans)
                .ThenInclude(l => l.Installments)
            .FirstOrDefaultAsync(c => c.IdentityNumber == identityNumber);
    }

    public async Task<Customer?> GetCustomerWithLoansAndInstallmentsAsync(int customerId)
    {
        return await _context.Customers
            .Include(c => c.Loans)
                .ThenInclude(l => l.Installments)
            .FirstOrDefaultAsync(c => c.Id == customerId);
    }

    public async Task AddAsync(Customer customer)
    {
        await _context.Customers.AddAsync(customer);
    }

    public void Update(Customer customer)
    {
        _context.Customers.Update(customer);
    }

    public void Delete(Customer customer)
    {
        _context.Customers.Remove(customer);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}