using DotnetCRUD.Models;
using Microsoft.EntityFrameworkCore.Storage;

namespace DotnetCRUD.Repositories;

public interface IBookingRepository
{
    Task<List<Booking>> GetAllAsync();
    Task<List<Booking>> GetByCustomerIdAsync(int customerId);
    Task<List<Booking>> GetByMechanicIdAsync(int mechanicId);
    Task<List<Booking>> GetHistoryByVehicleIdAsync(int vehicleId);
    Task<Booking?> GetByIdAsync(int id);
    Task<Booking?> GetByIdForCustomerAsync(int id, int customerId);
    Task<Booking> CreateWithSlotGuardAsync(Booking booking, int durationMinutes);
    Task UpdateAsync(Booking booking);
    Task<IDbContextTransaction> BeginTransactionAsync();
}
