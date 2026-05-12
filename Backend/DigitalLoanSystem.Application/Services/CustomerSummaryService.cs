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

        if (customer == null)
            return null;

        var allInstallments = customer.Loans.SelectMany(l => l.Installments).ToList();
        var now = DateTime.UtcNow;

        var summary = new CustomerSummaryDto
        {
            CustomerId = customer.Id,
            FullName = $"{customer.FirstName} {customer.LastName}",
        };

        summary.DelayedInstallmentCount = allInstallments
            .Count(i => i.Status != InstallmentStatus.Paid && i.DueDate < now);

        summary.TotalLoanDebt = allInstallments
            .Where(i => i.Status != InstallmentStatus.Paid)
            .Sum(i => i.Amount);

        decimal totalPrincipal = customer.Loans.Sum(l => l.PrincipalAmount);
        decimal totalPaid = allInstallments.Where(i => i.Status == InstallmentStatus.Paid).Sum(i => i.Amount);
        summary.RemainingPrincipal = Math.Max(0, totalPrincipal - (totalPaid * 0.8m));

        summary.PaidInstallments = allInstallments
            .Where(i => i.Status == InstallmentStatus.Paid)
            .Select(i => new InstallmentDetailDto
            {
                Id = i.Id,
                LoanId = i.LoanId,
                InstallmentNumber = i.InstallmentNumber,
                Amount = i.Amount,
                DueDate = i.DueDate,
                Status = "Ödendi"
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