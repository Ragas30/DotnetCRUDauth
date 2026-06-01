using DotnetCRUD.DTOs.Vehicle;
using DotnetCRUD.Models;
using DotnetCRUD.Repositories;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DotnetCRUD.Services;

public class VehicleService : IVehicleService
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public VehicleService(
        IVehicleRepository vehicleRepository,
        IHttpContextAccessor httpContextAccessor)
    {
        _vehicleRepository = vehicleRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<List<VehicleResponseDto>> GetMyVehiclesAsync()
    {
        var userId = GetCurrentUserId();
        var vehicles = await _vehicleRepository.GetByUserIdAsync(userId);

        return vehicles.Select(MapToResponse).ToList();
    }

    public async Task<VehicleResponseDto?> GetMyVehicleByIdAsync(int id)
    {
        var userId = GetCurrentUserId();
        var vehicle = await _vehicleRepository.GetByIdAndUserIdAsync(id, userId);
        return vehicle == null ? null : MapToResponse(vehicle);
    }

    public async Task<VehicleResponseDto> CreateAsync(CreateVehicleDto dto)
    {
        var userId = GetCurrentUserId();
        var existing = await _vehicleRepository.GetByPlateNumberAsync(dto.PlateNumber.Trim().ToUpperInvariant());
        if (existing != null)
        {
            throw new Exception("Nomor plat sudah terdaftar");
        }

        var vehicle = new Vehicle
        {
            UserId = userId,
            PlateNumber = dto.PlateNumber.Trim().ToUpperInvariant(),
            Brand = dto.Brand.Trim(),
            Model = dto.Model.Trim(),
            Year = dto.Year,
            CurrentMileage = dto.CurrentMileage,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId.ToString()
        };

        await _vehicleRepository.CreateAsync(vehicle);
        return MapToResponse(vehicle);
    }

    public async Task<VehicleResponseDto?> UpdateAsync(int id, UpdateVehicleDto dto)
    {
        var userId = GetCurrentUserId();
        var vehicle = await _vehicleRepository.GetByIdAndUserIdAsync(id, userId);
        if (vehicle == null)
        {
            return null;
        }

        var normalizedPlate = dto.PlateNumber.Trim().ToUpperInvariant();
        var plateOwner = await _vehicleRepository.GetByPlateNumberAsync(normalizedPlate);
        if (plateOwner != null && plateOwner.Id != id)
        {
            throw new Exception("Nomor plat sudah dipakai kendaraan lain");
        }

        vehicle.PlateNumber = normalizedPlate;
        vehicle.Brand = dto.Brand.Trim();
        vehicle.Model = dto.Model.Trim();
        vehicle.Year = dto.Year;
        vehicle.CurrentMileage = dto.CurrentMileage;
        vehicle.UpdatedAt = DateTime.UtcNow;
        vehicle.UpdatedBy = userId.ToString();

        await _vehicleRepository.UpdateAsync(vehicle);
        return MapToResponse(vehicle);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var userId = GetCurrentUserId();
        var vehicle = await _vehicleRepository.GetByIdAndUserIdAsync(id, userId);
        if (vehicle == null)
        {
            return false;
        }

        await _vehicleRepository.DeleteAsync(vehicle);
        return true;
    }

    private int GetCurrentUserId()
    {
        var userIdValue = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdValue, out var userId))
        {
            throw new Exception("User tidak valid");
        }

        return userId;
    }

    private static VehicleResponseDto MapToResponse(Vehicle vehicle)
    {
        return new VehicleResponseDto
        {
            Id = vehicle.Id,
            UserId = vehicle.UserId,
            PlateNumber = vehicle.PlateNumber,
            Brand = vehicle.Brand,
            Model = vehicle.Model,
            Year = vehicle.Year,
            CurrentMileage = vehicle.CurrentMileage
        };
    }
}
