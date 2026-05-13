namespace DigitalLoanSystem.Core.Entities;

public class Customer
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string IdentityNumber { get; set; } = string.Empty;
    public int CreditScore { get; set; } = 1200; // Varsayılan/Başlangıç kredi skoru
    public decimal Balance { get; set; } = 0m;

    public ICollection<Loan> Loans { get; set; } = new List<Loan>();
}