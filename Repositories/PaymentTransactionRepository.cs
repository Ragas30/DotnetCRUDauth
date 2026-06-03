using DotnetCRUD.Data;
using DotnetCRUD.Models;
using Microsoft.EntityFrameworkCore;

namespace DotnetCRUD.Repositories;

public class PaymentTransactionRepository : IPaymentTransactionRepository
{
    private readonly AppDbContext _context;

    public PaymentTransactionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PaymentTransaction> CreateAsync(PaymentTransaction paymentTransaction)
    {
        _context.PaymentTransactions.Add(paymentTransaction);
        await _context.SaveChangesAsync();
        return paymentTransaction;
    }

    public async Task<PaymentTransaction?> GetLatestByBookingIdAsync(int bookingId)
    {
        return await _context.PaymentTransactions
            .Where(p => p.BookingId == bookingId)
            .OrderByDescending(p => p.CreatedAt)
            .FirstOrDefaultAsync();
    }
}
