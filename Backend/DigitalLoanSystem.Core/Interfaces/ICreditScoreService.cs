namespace DigitalLoanSystem.Core.Interfaces;

public interface ICreditScoreService
{
    Task<int> GetCreditScoreAsync(string identityNumber);
}