using DigitalLoanSystem.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace DigitalLoanSystem.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Customer> Customers { get; set; }
    public DbSet<Loan> Loans { get; set; }
    public DbSet<Installment> Installments { get; set; }
    public DbSet<Payment> Payments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Loan>().Property(x => x.PrincipalAmount).HasPrecision(18, 2);
        modelBuilder.Entity<Loan>().Property(x => x.InterestRate).HasPrecision(5, 2);
        modelBuilder.Entity<Installment>().Property(x => x.Amount).HasPrecision(18, 2);
        modelBuilder.Entity<Payment>().Property(x => x.Amount).HasPrecision(18, 2);

        modelBuilder.Entity<Customer>()
            .HasMany(c => c.Loans)
            .WithOne(l => l.Customer)
            .HasForeignKey(l => l.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Loan>()
            .HasMany(l => l.Installments)
            .WithOne(i => i.Loan)
            .HasForeignKey(i => i.LoanId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Installment>()
            .HasOne(i => i.Payment)
            .WithOne(p => p.Installment)
            .HasForeignKey<Payment>(p => p.InstallmentId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}