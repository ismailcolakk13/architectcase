using DigitalLoanSystem.Application.DTOs;
using DigitalLoanSystem.Core.Enums;
using DigitalLoanSystem.Core.Interfaces;

namespace DigitalLoanSystem.Application.Services;

public class CustomerSummaryService : ICustomerSummaryService
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerSummaryService(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<CustomerSummaryDto?> GetCustomerSummaryAsync(int customerId)
    {
        var customer = await _customerRepository.GetCustomerWithLoansAndInstallmentsAsync(customerId);
        if (customer == null) return null;

        var now = DateTime.UtcNow;
        var allInstallments = customer.Loans.SelectMany(l => l.Installments).ToList();

        var summary = new CustomerSummaryDto
        {
            CustomerId = customer.Id,
            FullName = $"{customer.FirstName} {customer.LastName}",
            CreditScore = customer.CreditScore,
            Balance = customer.Balance
        };

        // Gecikmiş taksit sayısı (DueDate geçmiş ve ödenmemiş)
        summary.DelayedInstallmentCount = allInstallments
            .Count(i => i.Status != InstallmentStatus.Paid && i.DueDate < now);

        // Toplam kalan borç: ödenmeyen taksitlerin toplamı
        summary.TotalLoanDebt = allInstallments
            .Where(i => i.Status != InstallmentStatus.Paid)
            .Sum(i => i.Amount);

        // Kalan anapara: her kredi için anapara × (kalan taksit / toplam taksit)
        summary.RemainingPrincipal = Math.Round(
            customer.Loans.Sum(loan =>
            {
                int total = loan.Installments.Count;
                if (total == 0) return 0m;
                int unpaid = loan.Installments.Count(i => i.Status != InstallmentStatus.Paid);
                return loan.PrincipalAmount * ((decimal)unpaid / total);
            }), 2);

        summary.PaidInstallments = allInstallments
            .Where(i => i.Status == InstallmentStatus.Paid)
            .Select(i => new InstallmentDetailDto
            {
                Id = i.Id,
                LoanId = i.LoanId,
                InstallmentNumber = i.InstallmentNumber,
                Amount = i.Amount,
                DueDate = i.DueDate,
                Status = "Ödendi",
                PaymentDate = i.Payment?.PaymentDate
            }).ToList();

        summary.UnpaidInstallments = allInstallments
            .Where(i => i.Status != InstallmentStatus.Paid)
            .Select(i => new InstallmentDetailDto
            {
                Id = i.Id,
                LoanId = i.LoanId,
                InstallmentNumber = i.InstallmentNumber,
                Amount = i.Amount,
                DueDate = i.DueDate,
                Status = i.DueDate < now ? "Gecikmiş" : "Ödenmedi"
            }).ToList();

        return summary;
    }
}