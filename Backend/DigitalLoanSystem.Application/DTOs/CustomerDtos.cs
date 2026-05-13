using System.ComponentModel.DataAnnotations;

namespace DigitalLoanSystem.Application.DTOs;

public class CreateCustomerDto
{
    [Required] public string FirstName { get; set; } = string.Empty;
    [Required] public string LastName { get; set; } = string.Empty;
    [Required, EmailAddress] public string Email { get; set; } = string.Empty;
    [Required] public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    [Required] public string IdentityNumber { get; set; } = string.Empty;
    public int CreditScore { get; set; } = 1200;
}

public class UpdateCustomerDto
{
    [Required] public string FirstName { get; set; } = string.Empty;
    [Required] public string LastName { get; set; } = string.Empty;
    [Required, EmailAddress] public string Email { get; set; } = string.Empty;
    [Required] public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public int CreditScore { get; set; }
}
