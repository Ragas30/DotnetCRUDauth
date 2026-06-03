using DotnetCRUD.Models;

namespace DotnetCRUD.DTOs.Booking;

public class BookingInvoiceDto
{
    public int BookingId { get; set; }
    public string PlateNumber { get; set; } = string.Empty;
    public string ServiceName { get; set; } = string.Empty;
    public DateTime BookingDateTime { get; set; }
    public DateTime? CompletedAt { get; set; }
    public decimal EstimatedCost { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public PaymentMethod? PaymentMethod { get; set; }
    public decimal? PaidAmount { get; set; }
    public DateTime? PaidAt { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? ServiceNotes { get; set; }
}
