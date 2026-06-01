using DotnetCRUD.Models;

namespace DotnetCRUD.Repositories;

public interface IServiceCatalogRepository
{
    Task<List<ServiceCatalog>> GetAllAsync(bool activeOnly = false);
    Task<ServiceCatalog?> GetByIdAsync(int id);
    Task<ServiceCatalog?> GetByNameAsync(string name);
    Task<ServiceCatalog> CreateAsync(ServiceCatalog serviceCatalog);
    Task UpdateAsync(ServiceCatalog serviceCatalog);
    Task DeleteAsync(ServiceCatalog serviceCatalog);
}
