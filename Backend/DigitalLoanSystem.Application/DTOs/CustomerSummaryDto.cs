namespace DigitalLoanSystem.Application.DTOs;

public class InstallmentDetailDto
{
    public int Id { get; set; }
    public int LoanId { get; set; }
    public int InstallmentNumber { get; set; }
    public decimal Amount { get; set; }
    public DateTime DueDate { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class CustomerSummaryDto
{
    public int CustomerId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public decimal TotalLoanDebt { get; set; }
    public decimal RemainingPrincipal { get; set; }
    public int DelayedInstallmentCount { get; set; }

    public List<InstallmentDetailDto> PaidInstallments { get; set; } = new();
    public List<InstallmentDetailDto> UnpaidInstallments { get; set; } = new();
}