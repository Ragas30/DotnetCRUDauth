namespace DotnetCRUD.Models;

public class Vehicle : AuditableEntity
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string PlateNumber { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public int CurrentMileage { get; set; }
    public bool IsDeleted { get; set; }

    public User? User { get; set; }
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
