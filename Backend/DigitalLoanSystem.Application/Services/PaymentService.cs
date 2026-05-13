using DigitalLoanSystem.Application.DTOs;
using DigitalLoanSystem.Core.Entities;
using DigitalLoanSystem.Core.Enums;
using DigitalLoanSystem.Core.Interfaces;

namespace DigitalLoanSystem.Application.Services;

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly ILoanRepository _loanRepository;

    public PaymentService(IPaymentRepository paymentRepository, ILoanRepository loanRepository)
    {
        _paymentRepository = paymentRepository;
        _loanRepository = loanRepository;
    }

    public async Task<Payment> CreatePaymentAsync(CreatePaymentDto dto)
    {
        var installment = await _paymentRepository.GetInstallmentByIdAsync(dto.InstallmentId);

        if (installment == null)
            throw new ArgumentException("Belirtilen taksit bulunamadı.");

        if (installment.Status == InstallmentStatus.Paid)
            throw new InvalidOperationException("Bu taksit daha önce ödenmiş.");

        if (installment.Amount != dto.Amount)
            throw new ArgumentException($"Ödeme tutarı hatalı. Beklenen tutar: {installment.Amount:C2}");

        var customer = installment.Loan.Customer;
        if (customer.Balance < dto.Amount)
            throw new InvalidOperationException($"Yetersiz bakiye. Mevcut bakiyeniz: {customer.Balance:C2}, Ödenmesi gereken: {dto.Amount:C2}");

        // Bakiyeden düş
        customer.Balance -= dto.Amount;

        installment.Status = InstallmentStatus.Paid;

        var payment = new Payment
        {
            InstallmentId = installment.Id,
            Amount = dto.Amount,
            PaymentDate = DateTime.UtcNow
        };

        var savedPayment = await _paymentRepository.SavePaymentAndUpdateInstallmentAsync(payment, installment);

        // Tüm taksitler ödendiyse krediyi kapat
        var loan = await _loanRepository.GetByIdWithInstallmentsAsync(installment.LoanId);
        if (loan != null && loan.Status == LoanStatus.Active
            && loan.Installments.All(i => i.Status == InstallmentStatus.Paid))
        {
            loan.Status = LoanStatus.Closed;
            _loanRepository.Update(loan);
            await _loanRepository.SaveChangesAsync();
        }

        return savedPayment;
    }

    public async Task<IEnumerable<Payment>> GetAllPaymentsAsync() => await _paymentRepository.GetAllAsync();

    public async Task<Payment?> GetPaymentByIdAsync(int id) => await _paymentRepository.GetByIdAsync(id);

    public async Task<Payment?> GetPaymentByInstallmentIdAsync(int installmentId)
        => await _paymentRepository.GetByInstallmentIdAsync(installmentId);
}