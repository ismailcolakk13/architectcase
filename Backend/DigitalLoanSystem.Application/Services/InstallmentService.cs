using DigitalLoanSystem.Core.Entities;
using DigitalLoanSystem.Core.Enums;
using DigitalLoanSystem.Core.Interfaces;

namespace DigitalLoanSystem.Application.Services;

public class InstallmentService : IInstallmentService
{
    private readonly IInstallmentRepository _installmentRepository;

    public InstallmentService(IInstallmentRepository installmentRepository)
    {
        _installmentRepository = installmentRepository;
    }

    public async Task<Installment?> GetInstallmentByIdAsync(int id)
    {
        var installment = await _installmentRepository.GetByIdAsync(id);
        if (installment != null)
            await SyncDelayedStatusAsync(new[] { installment });
        return installment;
    }

    public async Task<IEnumerable<Installment>> GetInstallmentsByLoanIdAsync(int loanId)
    {
        var installments = await _installmentRepository.GetByLoanIdAsync(loanId);
        await SyncDelayedStatusAsync(installments);
        return installments;
    }

    public async Task<IEnumerable<Installment>> GetInstallmentsByCustomerIdAsync(int customerId)
    {
        var installments = await _installmentRepository.GetByCustomerIdAsync(customerId);
        await SyncDelayedStatusAsync(installments);
        return installments;
    }

    /// <summary>
    /// Vadesi geçmiş Unpaid taksitleri Delayed olarak DB'ye yazar (on-read güncelleme).
    /// </summary>
    private async Task SyncDelayedStatusAsync(IEnumerable<Installment> installments)
    {
        var now = DateTime.UtcNow;
        bool hasChanges = false;

        foreach (var inst in installments)
        {
            if (inst.Status == InstallmentStatus.Unpaid && inst.DueDate < now)
            {
                inst.Status = InstallmentStatus.Delayed;
                hasChanges = true;
            }
        }

        if (hasChanges)
            await _installmentRepository.SaveChangesAsync();
    }
}
