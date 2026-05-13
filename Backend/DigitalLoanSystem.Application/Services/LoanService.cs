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

    // Kredi skoru eşik değeri — yapılandırmadan okunabilir, şimdilik sabit.
    private const int MinCreditScore = 1000;

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
            throw new ArgumentException("Müşteri bulunamadı.");

        int creditScore = await _creditScoreService.GetCreditScoreAsync(customer.IdentityNumber);
        if (creditScore < MinCreditScore)
            throw new InvalidOperationException($"Kredi skoru yetersiz: {creditScore}. Başvuru reddedildi.");

        // Kategori bazlı faiz ve vade doğrulaması (Backend Validation)
        var (expectedRate, expectedTerm) = dto.Type switch
        {
            LoanType.Personal => (18.0m, 12),
            LoanType.Education => (12.0m, 12),
            LoanType.Vehicle => (24.0m, 12),
            LoanType.Housing => (30.0m, 12),
            LoanType.Business => (21.0m, 6),
            _ => (18.0m, 12)
        };

        if (dto.InterestRate != expectedRate || dto.TermInMonths != expectedTerm)
            throw new ArgumentException("Kredi faiz oranı veya vade süresi seçilen kategori ile uyuşmuyor.");

        var loan = new Loan
        {
            CustomerId = dto.CustomerId,
            Type = dto.Type,
            PrincipalAmount = dto.PrincipalAmount,
            InterestRate = dto.InterestRate,
            TermInMonths = dto.TermInMonths,
            StartDate = DateTime.UtcNow,
            Status = LoanStatus.Pending, // Başlangıçta Onay Bekliyor durumunda
            Installments = new List<Installment>()
        };

        // Yıllık faiz oranını aylık faize çevir
        decimal monthlyRate = dto.InterestRate / 100m / 12m;
        decimal monthlyInstallmentAmount;

        if (monthlyRate > 0)
        {
            double r = (double)monthlyRate;
            double power = Math.Pow(1 + r, dto.TermInMonths);
            monthlyInstallmentAmount = dto.PrincipalAmount * (decimal)(r * power / (power - 1));
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

    public async Task<IEnumerable<Loan>> GetLoansByCustomerIdAsync(int customerId)
        => await _loanRepository.GetByCustomerIdAsync(customerId);

    public async Task UpdateLoanAsync(int id, UpdateLoanDto dto)
    {
        var loan = await _loanRepository.GetByIdWithInstallmentsAsync(id);
        if (loan == null)
            throw new ArgumentException("Kredi bulunamadı.");

        loan.Status = dto.Status;
        _loanRepository.Update(loan);
        await _loanRepository.SaveChangesAsync();
    }
}