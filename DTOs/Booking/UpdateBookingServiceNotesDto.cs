namespace DotnetCRUD.DTOs.Booking;

public class UpdateBookingServiceNotesDto
{
    public string ServiceNotes { get; set; } = string.Empty;
    public DateTime? RecommendedNextServiceDate { get; set; }
    public int? RecommendedNextServiceMileage { get; set; }
}
