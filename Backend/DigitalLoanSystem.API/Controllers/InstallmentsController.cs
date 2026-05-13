using DigitalLoanSystem.Application.Services;
using DigitalLoanSystem.Core.Entities;
using Microsoft.AspNetCore.Mvc;

namespace DigitalLoanSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InstallmentsController : ControllerBase
{
    private readonly IInstallmentService _installmentService;

    public InstallmentsController(IInstallmentService installmentService)
    {
        _installmentService = installmentService;
    }

    /// <summary>
    /// Taksit detayını getirir. Kullanıcı kendi taksitini, yönetici herhangi bir taksiti görebilir.
    /// GET: api/installments/{id}
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<Installment>> GetInstallment(int id)
    {
        var installment = await _installmentService.GetInstallmentByIdAsync(id);
        if (installment == null)
            return NotFound(new { Message = "Taksit bulunamadı." });

        return Ok(installment);
    }

    /// <summary>
    /// Belirli bir krediye ait tüm taksitleri listeler.
    /// Kullanıcı: kendi kredisinin taksitlerini görür.
    /// GET: api/installments/by-loan/{loanId}
    /// </summary>
    [HttpGet("by-loan/{loanId}")]
    public async Task<ActionResult<IEnumerable<Installment>>> GetByLoan(int loanId)
    {
        var installments = await _installmentService.GetInstallmentsByLoanIdAsync(loanId);
        return Ok(installments);
    }

    /// <summary>
    /// Bir müşteriye ait tüm kredilerin taksitlerini listeler.
    /// Yönetim paneli için: her müşterinin tüm taksit tablosu.
    /// GET: api/installments/by-customer/{customerId}
    /// </summary>
    [HttpGet("by-customer/{customerId}")]
    public async Task<ActionResult<IEnumerable<Installment>>> GetByCustomer(int customerId)
    {
        var installments = await _installmentService.GetInstallmentsByCustomerIdAsync(customerId);
        return Ok(installments);
    }
}
