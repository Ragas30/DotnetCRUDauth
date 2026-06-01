using DotnetCRUD.DTOs.ServiceCatalog;
using DotnetCRUD.Models;
using DotnetCRUD.Repositories;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DotnetCRUD.Services;

public class ServiceCatalogService : IServiceCatalogService
{
    private readonly IServiceCatalogRepository _serviceCatalogRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ServiceCatalogService(
        IServiceCatalogRepository serviceCatalogRepository,
        IHttpContextAccessor httpContextAccessor)
    {
        _serviceCatalogRepository = serviceCatalogRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<List<ServiceCatalogResponseDto>> GetAllAsync(bool activeOnly)
    {
        var services = await _serviceCatalogRepository.GetAllAsync(activeOnly);
        return services.Select(MapToResponse).ToList();
    }

    public async Task<ServiceCatalogResponseDto?> GetByIdAsync(int id)
    {
        var serviceCatalog = await _serviceCatalogRepository.GetByIdAsync(id);
        return serviceCatalog == null ? null : MapToResponse(serviceCatalog);
    }

    public async Task<ServiceCatalogResponseDto> CreateAsync(CreateServiceCatalogDto dto)
    {
        var existing = await _serviceCatalogRepository.GetByNameAsync(dto.Name.Trim());
        if (existing != null)
        {
            throw new Exception("Nama layanan sudah terdaftar");
        }

        var actor = GetActorIdentity();

        var entity = new ServiceCatalog
        {
            Name = dto.Name.Trim(),
            DurationMinutes = dto.DurationMinutes,
            BasePrice = dto.BasePrice,
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = actor
        };

        await _serviceCatalogRepository.CreateAsync(entity);
        return MapToResponse(entity);
    }

    public async Task<ServiceCatalogResponseDto?> UpdateAsync(int id, UpdateServiceCatalogDto dto)
    {
        var serviceCatalog = await _serviceCatalogRepository.GetByIdAsync(id);
        if (serviceCatalog == null)
        {
            return null;
        }

        var existing = await _serviceCatalogRepository.GetByNameAsync(dto.Name.Trim());
        if (existing != null && existing.Id != id)
        {
            throw new Exception("Nama layanan sudah dipakai layanan lain");
        }

        serviceCatalog.Name = dto.Name.Trim();
        serviceCatalog.DurationMinutes = dto.DurationMinutes;
        serviceCatalog.BasePrice = dto.BasePrice;
        serviceCatalog.IsActive = dto.IsActive;
        serviceCatalog.UpdatedAt = DateTime.UtcNow;
        serviceCatalog.UpdatedBy = GetActorIdentity();

        await _serviceCatalogRepository.UpdateAsync(serviceCatalog);
        return MapToResponse(serviceCatalog);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var serviceCatalog = await _serviceCatalogRepository.GetByIdAsync(id);
        if (serviceCatalog == null)
        {
            return false;
        }

        await _serviceCatalogRepository.DeleteAsync(serviceCatalog);
        return true;
    }

    private string GetActorIdentity()
    {
        return _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Email)
            ?? _httpContextAccessor.HttpContext?.User.Identity?.Name
            ?? "system";
    }

    private static ServiceCatalogResponseDto MapToResponse(ServiceCatalog serviceCatalog)
    {
        return new ServiceCatalogResponseDto
        {
            Id = serviceCatalog.Id,
            Name = serviceCatalog.Name,
            DurationMinutes = serviceCatalog.DurationMinutes,
            BasePrice = serviceCatalog.BasePrice,
            IsActive = serviceCatalog.IsActive
        };
    }
}
