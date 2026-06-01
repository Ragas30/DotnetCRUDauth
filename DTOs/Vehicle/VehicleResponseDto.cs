namespace DotnetCRUD.DTOs.Vehicle;

public class VehicleResponseDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string PlateNumber { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public int CurrentMileage { get; set; }
}
