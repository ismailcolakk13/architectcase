using DigitalLoanSystem.Application.DTOs;
using DigitalLoanSystem.Application.Services;
using DigitalLoanSystem.Core.Entities;
using Microsoft.AspNetCore.Mvc;

namespace DigitalLoanSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    // GET: api/payments
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Payment>>> GetPayments()
    {
        var payments = await _paymentService.GetAllPaymentsAsync();
        return Ok(payments);
    }

    // GET: api/payments/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Payment>> GetPayment(int id)
    {
        var payment = await _paymentService.GetPaymentByIdAsync(id);
        if (payment == null)
            return NotFound(new { Message = "Ödeme bulunamadı." });

        return Ok(payment);
    }

    // GET: api/payments/by-installment/{installmentId}
    [HttpGet("by-installment/{installmentId}")]
    public async Task<ActionResult<Payment>> GetPaymentByInstallment(int installmentId)
    {
        var payment = await _paymentService.GetPaymentByInstallmentIdAsync(installmentId);
        if (payment == null)
            return NotFound(new { Message = "Bu taksit için ödeme kaydı bulunamadı." });

        return Ok(payment);
    }

    // POST: api/payments
    [HttpPost]
    public async Task<ActionResult<Payment>> CreatePayment(CreatePaymentDto dto)
    {
        try
        {
            var payment = await _paymentService.CreatePaymentAsync(dto);
            return CreatedAtAction(nameof(GetPayment), new { id = payment.Id }, payment);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Ödeme işlemi sırasında bir hata oluştu.", Details = ex.Message });
        }
    }
}