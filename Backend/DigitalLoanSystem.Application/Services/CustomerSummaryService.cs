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
        // Sadece aktif krediler üzerinden borç hesapla
        var activeLoans = customer.Loans.Where(l => l.Status == LoanStatus.Active).ToList();
        var activeInstallments = activeLoans.SelectMany(l => l.Installments).ToList();
        var allInstallments = customer.Loans.SelectMany(l => l.Installments).ToList();

        var summary = new CustomerSummaryDto
        {
            CustomerId = customer.Id,
            FullName = $"{customer.FirstName} {customer.LastName}",
            CreditScore = customer.CreditScore,
            Balance = customer.Balance
        };

        // Gecikmiş taksit sayısı (Sadece aktif kredilerde DueDate geçmiş ve ödenmemiş)
        summary.DelayedInstallmentCount = activeInstallments
            .Count(i => i.Status != InstallmentStatus.Paid && i.DueDate < now);

        // Toplam kalan borç: Aktif kredilerin ödenmeyen taksitlerinin toplamı
        summary.TotalLoanDebt = activeInstallments
            .Where(i => i.Status != InstallmentStatus.Paid)
            .Sum(i => i.Amount);

        // Kalan anapara: Sadece aktif krediler için
        summary.RemainingPrincipal = Math.Round(
            activeLoans.Sum(loan =>
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

        summary.UnpaidInstallments = activeInstallments
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