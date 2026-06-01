using DotnetCRUD.Data;
using DotnetCRUD.Models;
using Microsoft.EntityFrameworkCore;

namespace DotnetCRUD.Repositories;

public class VehicleRepository : IVehicleRepository
{
    private readonly AppDbContext _context;

    public VehicleRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Vehicle>> GetByUserIdAsync(int userId)
    {
        return await _context.Vehicles
            .Where(v => v.UserId == userId)
            .OrderByDescending(v => v.CreatedAt)
            .ToListAsync();
    }

    public async Task<Vehicle?> GetByIdAsync(int id)
    {
        return await _context.Vehicles.FindAsync(id);
    }

    public async Task<Vehicle?> GetByIdAndUserIdAsync(int id, int userId)
    {
        return await _context.Vehicles
            .FirstOrDefaultAsync(v => v.Id == id && v.UserId == userId);
    }

    public async Task<Vehicle?> GetByPlateNumberAsync(string plateNumber)
    {
        return await _context.Vehicles
            .FirstOrDefaultAsync(v => v.PlateNumber == plateNumber);
    }

    public async Task<Vehicle> CreateAsync(Vehicle vehicle)
    {
        _context.Vehicles.Add(vehicle);
        await _context.SaveChangesAsync();
        return vehicle;
    }

    public async Task UpdateAsync(Vehicle vehicle)
    {
        _context.Vehicles.Update(vehicle);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Vehicle vehicle)
    {
        _context.Vehicles.Remove(vehicle);
        await _context.SaveChangesAsync();
    }
}
