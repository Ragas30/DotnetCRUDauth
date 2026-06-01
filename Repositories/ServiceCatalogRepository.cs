using DotnetCRUD.Data;
using DotnetCRUD.Models;
using Microsoft.EntityFrameworkCore;

namespace DotnetCRUD.Repositories;

public class ServiceCatalogRepository : IServiceCatalogRepository
{
    private readonly AppDbContext _context;

    public ServiceCatalogRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<ServiceCatalog>> GetAllAsync(bool activeOnly = false)
    {
        var query = _context.ServiceCatalogs.AsQueryable();
        if (activeOnly)
        {
            query = query.Where(s => s.IsActive);
        }

        return await query
            .OrderBy(s => s.Name)
            .ToListAsync();
    }

    public async Task<ServiceCatalog?> GetByIdAsync(int id)
    {
        return await _context.ServiceCatalogs.FindAsync(id);
    }

    public async Task<ServiceCatalog?> GetByNameAsync(string name)
    {
        return await _context.ServiceCatalogs
            .FirstOrDefaultAsync(s => s.Name.ToLower() == name.ToLower());
    }

    public async Task<ServiceCatalog> CreateAsync(ServiceCatalog serviceCatalog)
    {
        _context.ServiceCatalogs.Add(serviceCatalog);
        await _context.SaveChangesAsync();
        return serviceCatalog;
    }

    public async Task UpdateAsync(ServiceCatalog serviceCatalog)
    {
        _context.ServiceCatalogs.Update(serviceCatalog);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(ServiceCatalog serviceCatalog)
    {
        _context.ServiceCatalogs.Remove(serviceCatalog);
        await _context.SaveChangesAsync();
    }
}
