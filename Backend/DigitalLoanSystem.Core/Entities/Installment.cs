using DigitalLoanSystem.Core.Enums;

namespace DigitalLoanSystem.Core.Entities;

public class Installment
{
    public int Id { get; set; }
    public int LoanId { get; set; }

    public int InstallmentNumber { get; set; }
    public decimal Amount { get; set; }
    public DateTime DueDate { get; set; }
    public InstallmentStatus Status { get; set; }

    public Loan Loan { get; set; } = null!;

    public Payment? Payment { get; set; }
}