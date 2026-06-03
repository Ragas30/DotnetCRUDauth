using DotnetCRUD.Models;

namespace DotnetCRUD.DTOs.Booking;

public class BookingHistoryResponseDto
{
    public int BookingId { get; set; }
    public int VehicleId { get; set; }
    public string PlateNumber { get; set; } = string.Empty;
    public string ServiceName { get; set; } = string.Empty;
    public DateTime BookingDateTime { get; set; }
    public BookingStatus Status { get; set; }
    public decimal? EstimatedCost { get; set; }
    public string? ServiceNotes { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? RecommendedNextServiceDate { get; set; }
    public int? RecommendedNextServiceMileage { get; set; }
}
