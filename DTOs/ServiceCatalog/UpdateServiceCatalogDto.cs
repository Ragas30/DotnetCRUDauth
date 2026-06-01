namespace DotnetCRUD.DTOs.ServiceCatalog;

public class UpdateServiceCatalogDto
{
    public string Name { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public decimal BasePrice { get; set; }
    public bool IsActive { get; set; } = true;
}
