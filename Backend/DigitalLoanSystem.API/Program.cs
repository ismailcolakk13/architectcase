using System.Text.Json.Serialization;
using DigitalLoanSystem.Application.Services;
using DigitalLoanSystem.Core.Interfaces;
using DigitalLoanSystem.Infrastructure.Data;
using DigitalLoanSystem.Infrastructure.ExternalServices;
using DigitalLoanSystem.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Veritabanı Bağlantısı
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// 2. Controller Desteğini Ekle
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

// 3. Repository kayıtları
builder.Services.AddScoped<ILoanRepository, LoanRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IInstallmentRepository, InstallmentRepository>();

// 4. Service kayıtları
builder.Services.AddScoped<ILoanService, LoanService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<ICustomerSummaryService, CustomerSummaryService>();
builder.Services.AddScoped<IInstallmentService, InstallmentService>();

// 5. Dış servis entegrasyonu (Mock)
builder.Services.AddScoped<ICreditScoreService, MockCreditScoreService>();

// OpenAPI/Swagger Desteği
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// 6. Gelen istekleri Controller'lara yönlendir
app.MapControllers();

app.Run();