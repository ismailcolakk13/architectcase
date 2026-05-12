using DigitalLoanSystem.Core.Interfaces;

namespace DigitalLoanSystem.Infrastructure.ExternalServices;

public class MockCreditScoreService : ICreditScoreService
{
    public async Task<int> GetCreditScoreAsync(string identityNumber)
    {
        await Task.Delay(500);

        int seed = string.IsNullOrEmpty(identityNumber) ? Guid.NewGuid().GetHashCode() : identityNumber.GetHashCode();
        var random = new Random(seed);

        return random.Next(500, 1901);
    }
}