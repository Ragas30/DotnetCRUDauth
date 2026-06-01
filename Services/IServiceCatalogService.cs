using DotnetCRUD.DTOs.ServiceCatalog;

namespace DotnetCRUD.Services;

public interface IServiceCatalogService
{
    Task<List<ServiceCatalogResponseDto>> GetAllAsync(bool activeOnly);
    Task<ServiceCatalogResponseDto?> GetByIdAsync(int id);
    Task<ServiceCatalogResponseDto> CreateAsync(CreateServiceCatalogDto dto);
    Task<ServiceCatalogResponseDto?> UpdateAsync(int id, UpdateServiceCatalogDto dto);
    Task<bool> DeleteAsync(int id);
}
