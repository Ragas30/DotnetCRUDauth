using DotnetCRUD.Models;

namespace DotnetCRUD.Repositories;

public interface IPaymentTransactionRepository
{
    Task<PaymentTransaction> CreateAsync(PaymentTransaction paymentTransaction);
    Task<PaymentTransaction?> GetLatestByBookingIdAsync(int bookingId);
}
