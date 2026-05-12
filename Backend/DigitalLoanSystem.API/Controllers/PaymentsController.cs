using DigitalLoanSystem.Application.DTOs;
using DigitalLoanSystem.Application.Services;
using DigitalLoanSystem.Core.Entities;
using DigitalLoanSystem.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DigitalLoanSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    // AppDbContext silindi.
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

    // POST: api/payments
    [HttpPost]
    public async Task<ActionResult<Payment>> CreatePayment(CreatePaymentDto dto)
    {
        try
        {
            var payment = await _paymentService.CreatePaymentAsync(dto);
            return Ok(payment);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

}