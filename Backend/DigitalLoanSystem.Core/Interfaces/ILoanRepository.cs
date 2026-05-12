using DigitalLoanSystem.Core.Entities;

namespace DigitalLoanSystem.Core.Interfaces;

public interface ILoanRepository
{
    Task<Loan> AddAsync(Loan loan);
    Task SaveChangesAsync();
    Task<IEnumerable<Loan>> GetAllAsync();
    Task<Loan?> GetByIdWithInstallmentsAsync(int id);
    void Update(Loan loan);
}