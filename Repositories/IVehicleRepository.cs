using DotnetCRUD.Models;

namespace DotnetCRUD.Repositories;

public interface IVehicleRepository
{
    Task<List<Vehicle>> GetByUserIdAsync(int userId);
    Task<Vehicle?> GetByIdAsync(int id);
    Task<Vehicle?> GetByIdAndUserIdAsync(int id, int userId);
    Task<Vehicle?> GetByPlateNumberAsync(string plateNumber);
    Task<Vehicle> CreateAsync(Vehicle vehicle);
    Task UpdateAsync(Vehicle vehicle);
    Task DeleteAsync(Vehicle vehicle);
}
