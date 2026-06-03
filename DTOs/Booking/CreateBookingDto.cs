namespace DotnetCRUD.DTOs.Booking;

public class CreateBookingDto
{
    public int VehicleId { get; set; }
    public int ServiceCatalogId { get; set; }
    public DateTime BookingDateTime { get; set; }
    public string Complaint { get; set; } = string.Empty;
}
