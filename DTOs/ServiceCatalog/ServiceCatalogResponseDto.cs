namespace DotnetCRUD.DTOs.ServiceCatalog;

public class ServiceCatalogResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public decimal BasePrice { get; set; }
    public bool IsActive { get; set; }
}
