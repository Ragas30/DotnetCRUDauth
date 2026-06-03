using DotnetCRUD.Models;

namespace DotnetCRUD.DTOs.Booking;

public class ManualPaymentDto
{
    public PaymentMethod PaymentMethod { get; set; }
    public decimal PaidAmount { get; set; }
    public string? ReferenceNumber { get; set; }
}
