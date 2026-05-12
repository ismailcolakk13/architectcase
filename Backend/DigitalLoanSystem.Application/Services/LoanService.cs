using DigitalLoanSystem.Application.DTOs;
using DigitalLoanSystem.Core.Entities;
using DigitalLoanSystem.Core.Enums;
using DigitalLoanSystem.Core.Interfaces;

namespace DigitalLoanSystem.Application.Services;

public class LoanService : ILoanService
{
    private readonly ILoanRepository _loanRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly ICreditScoreService _creditScoreService;

    public LoanService(ILoanRepository loanRepository, ICustomerRepository customerRepository, ICreditScoreService creditScoreService)
    {
        _loanRepository = loanRepository;
        _customerRepository = customerRepository;
        _creditScoreService = creditScoreService;
    }

    public async Task<Loan> CreateLoanWithInstallmentsAsync(CreateLoanDto dto)
    {
        var customer = await _customerRepository.GetCustomerWithLoansAndInstallmentsAsync(dto.CustomerId);
        if (customer == null)
        {
            throw new ArgumentException("Müşteri bulunamadı");
        }

        int creditScore = await _creditScoreService.GetCreditScoreAsync(customer.IdentityNumber);

        if (creditScore < 1000)
        {
            throw new InvalidOperationException($"Kredi skoru yetersiz: {creditScore} , başvuru reddedildi.");
        }

        var loan = new Loan
        {
            CustomerId = dto.CustomerId,
            Type = dto.Type,
            PrincipalAmount = dto.PrincipalAmount,
            InterestRate = dto.InterestRate,
            TermInMonths = dto.TermInMonths,
            StartDate = DateTime.UtcNow,
            Status = LoanStatus.Active,
            Installments = new List<Installment>()
        };

        decimal monthlyRate = dto.InterestRate / 100m;
        decimal monthlyInstallmentAmount;

        if (monthlyRate > 0)
        {
            double rateDouble = (double)monthlyRate;
            double mathPower = Math.Pow(1 + rateDouble, dto.TermInMonths);
            monthlyInstallmentAmount = dto.PrincipalAmount * (decimal)((rateDouble * mathPower) / (mathPower - 1));
        }
        else
        {
            monthlyInstallmentAmount = dto.PrincipalAmount / dto.TermInMonths;
        }

        for (int i = 1; i <= dto.TermInMonths; i++)
        {
            loan.Installments.Add(new Installment
            {
                InstallmentNumber = i,
                Amount = Math.Round(monthlyInstallmentAmount, 2),
                DueDate = loan.StartDate.AddMonths(i),
                Status = InstallmentStatus.Unpaid
            });
        }

        await _loanRepository.AddAsync(loan);
        await _loanRepository.SaveChangesAsync();

        return loan;
    }

    public async Task<IEnumerable<Loan>> GetAllLoansAsync() => await _loanRepository.GetAllAsync();

    public async Task<Loan?> GetLoanByIdAsync(int id) => await _loanRepository.GetByIdWithInstallmentsAsync(id);

    public async Task UpdateLoanAsync(Loan loan)
    {
        _loanRepository.Update(loan);
        await _loanRepository.SaveChangesAsync();
    }
}