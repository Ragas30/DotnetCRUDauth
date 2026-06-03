using DotnetCRUD.Models;

namespace DotnetCRUD.Repositories;

public interface IBookingRepository
{
    Task<List<Booking>> GetAllAsync();
    Task<List<Booking>> GetByCustomerIdAsync(int customerId);
    Task<List<Booking>> GetByMechanicIdAsync(int mechanicId);
    Task<List<Booking>> GetHistoryByVehicleIdAsync(int vehicleId);
    Task<Booking?> GetByIdAsync(int id);
    Task<Booking?> GetByIdForCustomerAsync(int id, int customerId);
    Task<bool> IsTimeSlotTakenAsync(DateTime bookingDateTime, int serviceCatalogId);
    Task<Booking> CreateAsync(Booking booking);
    Task UpdateAsync(Booking booking);
}
