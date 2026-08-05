namespace DotnetCRUD.Models;

public class Booking : AuditableEntity
{
    public int Id { get; set; }
    public int VehicleId { get; set; }
    public int ServiceCatalogId { get; set; }
    public DateTime BookingDateTime { get; set; }
    public string Complaint { get; set; } = string.Empty;
    public BookingStatus Status { get; set; } = BookingStatus.BOOKED;
    public decimal? EstimatedCost { get; set; }
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.UNPAID;
    public string? ServiceNotes { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? RecommendedNextServiceDate { get; set; }
    public int? RecommendedNextServiceMileage { get; set; }
    public int? MechanicId { get; set; }
    public long Version { get; set; }

    public Vehicle? Vehicle { get; set; }
    public ServiceCatalog? ServiceCatalog { get; set; }
    public User? Mechanic { get; set; }
    public ICollection<PaymentTransaction> PaymentTransactions { get; set; } = new List<PaymentTransaction>();
}
