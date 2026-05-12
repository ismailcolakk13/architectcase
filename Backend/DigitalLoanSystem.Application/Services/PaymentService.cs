using DigitalLoanSystem.Application.DTOs;
using DigitalLoanSystem.Core.Entities;
using DigitalLoanSystem.Core.Enums;
using DigitalLoanSystem.Core.Interfaces;

namespace DigitalLoanSystem.Application.Services;

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _paymentRepository;

    public PaymentService(IPaymentRepository paymentRepository)
    {
        _paymentRepository = paymentRepository;
    }

    public async Task<Payment> CreatePaymentAsync(CreatePaymentDto dto)
    {
        var installment = await _paymentRepository.GetInstallmentByIdAsync(dto.InstallmentId);

        if (installment == null)
        {
            throw new ArgumentException("Belirtilen taksit bulunamadı.");
        }

        if (installment.Status == InstallmentStatus.Paid)
        {
            throw new InvalidOperationException("Bu taksit daha önce ödenmiş.");
        }

        if (installment.Amount != dto.Amount)
        {
            throw new ArgumentException($"Ödeme tutarı hatalı. Beklenen tutar: {installment.Amount} TL");
        }

        installment.Status = InstallmentStatus.Paid;

        var payment = new Payment
        {
            InstallmentId = installment.Id,
            Amount = dto.Amount,
            PaymentDate = DateTime.UtcNow
        };

        return await _paymentRepository.SavePaymentAndUpdateInstallmentAsync(payment, installment);
    }

    public async Task<IEnumerable<Payment>> GetAllPaymentsAsync() => await _paymentRepository.GetAllAsync();
}