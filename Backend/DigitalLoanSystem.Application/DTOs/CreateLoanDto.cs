using DigitalLoanSystem.Core.Enums;

namespace DigitalLoanSystem.Application.DTOs;

public class CreateLoanDto
{
    public int CustomerId { get; set; }
    public LoanType Type { get; set; }
    public decimal PrincipalAmount { get; set; }
    public decimal InterestRate { get; set; }
    public int TermInMonths { get; set; }
}