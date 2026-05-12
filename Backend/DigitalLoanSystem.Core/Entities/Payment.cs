namespace DigitalLoanSystem.Core.Entities;

public class Payment
{
    public int Id { get; set; }
    public int InstallmentId { get; set; }

    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }

    public Installment Installment { get; set; } = null!;
}