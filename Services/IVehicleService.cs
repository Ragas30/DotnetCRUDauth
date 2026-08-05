using DotnetCRUD.DTOs.Vehicle;

namespace DotnetCRUD.Services;

public interface IVehicleService
{
    Task<List<VehicleResponseDto>> GetMyVehiclesAsync();
    Task<VehicleResponseDto> GetMyVehicleByIdAsync(int id);
    Task<VehicleResponseDto> CreateAsync(CreateVehicleDto dto);
    Task<VehicleResponseDto> UpdateAsync(int id, UpdateVehicleDto dto);
    Task DeleteAsync(int id);
}
