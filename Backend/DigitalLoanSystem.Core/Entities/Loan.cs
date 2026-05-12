using DigitalLoanSystem.Core.Enums;

namespace DigitalLoanSystem.Core.Entities;

public class Loan
{
    public int Id { get; set; }
    public int CustomerId { get; set; }

    public LoanType Type { get; set; }
    public decimal PrincipalAmount { get; set; }
    public decimal InterestRate { get; set; }
    public int TermInMonths { get; set; }
    public DateTime StartDate { get; set; }
    public LoanStatus Status { get; set; }

    public Customer Customer { get; set; } = null!;
    public ICollection<Installment> Installments { get; set; } = new List<Installment>();
}