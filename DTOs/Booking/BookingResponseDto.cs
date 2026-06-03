using DotnetCRUD.Models;

namespace DotnetCRUD.DTOs.Booking;

public class BookingResponseDto
{
    public int Id { get; set; }
    public int VehicleId { get; set; }
    public string PlateNumber { get; set; } = string.Empty;
    public int ServiceCatalogId { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public DateTime BookingDateTime { get; set; }
    public string Complaint { get; set; } = string.Empty;
    public BookingStatus Status { get; set; }
    public decimal? EstimatedCost { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public string? ServiceNotes { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? RecommendedNextServiceDate { get; set; }
    public int? RecommendedNextServiceMileage { get; set; }
    public int? MechanicId { get; set; }
}
