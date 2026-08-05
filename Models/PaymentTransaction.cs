namespace DotnetCRUD.Models;

public class PaymentTransaction : AuditableEntity
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public string Provider { get; set; } = "MANUAL";
    public string? ProviderTransactionId { get; set; }
    public string? ProviderOrderId { get; set; }
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.CASH;
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.UNPAID;
    public decimal Amount { get; set; }
    public string? ReferenceNumber { get; set; }
    public DateTime? PaidAt { get; set; }
    public string? RawNotificationPayload { get; set; }
    public long Version { get; set; }

    public Booking? Booking { get; set; }
}
