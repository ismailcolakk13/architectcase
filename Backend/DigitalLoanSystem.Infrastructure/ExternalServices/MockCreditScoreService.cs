using DigitalLoanSystem.Core.Interfaces;

namespace DigitalLoanSystem.Infrastructure.ExternalServices;

public class MockCreditScoreService : ICreditScoreService
{
    private readonly ICustomerRepository _customerRepository;

    public MockCreditScoreService(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<int> GetCreditScoreAsync(string identityNumber)
    {
        await Task.Delay(200); // Gerçekçi ağ gecikmesi simülasyonu

        var customer = await _customerRepository.GetByIdentityNumberAsync(identityNumber);
        if (customer != null)
        {
            return customer.CreditScore;
        }

        return 1200; // Bulunamazsa varsayılan
    }
}