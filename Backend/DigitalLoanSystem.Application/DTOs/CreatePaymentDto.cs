namespace DigitalLoanSystem.Application.DTOs;

public class CreatePaymentDto
{
    public int InstallmentId { get; set; }
    public decimal Amount { get; set; }
}