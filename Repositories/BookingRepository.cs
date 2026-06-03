using DotnetCRUD.Data;
using DotnetCRUD.Models;
using Microsoft.EntityFrameworkCore;

namespace DotnetCRUD.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly AppDbContext _context;

    public BookingRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Booking>> GetAllAsync()
    {
        return await QueryWithIncludes()
            .OrderByDescending(b => b.BookingDateTime)
            .ToListAsync();
    }

    public async Task<List<Booking>> GetByCustomerIdAsync(int customerId)
    {
        return await QueryWithIncludes()
            .Where(b => b.Vehicle != null && b.Vehicle.UserId == customerId)
            .OrderByDescending(b => b.BookingDateTime)
            .ToListAsync();
    }

    public async Task<List<Booking>> GetByMechanicIdAsync(int mechanicId)
    {
        return await QueryWithIncludes()
            .Where(b => b.MechanicId == mechanicId)
            .OrderByDescending(b => b.BookingDateTime)
            .ToListAsync();
    }

    public async Task<List<Booking>> GetHistoryByVehicleIdAsync(int vehicleId)
    {
        return await QueryWithIncludes()
            .Where(b => b.VehicleId == vehicleId && (b.Status == BookingStatus.DONE || b.Status == BookingStatus.PAID))
            .OrderByDescending(b => b.BookingDateTime)
            .ToListAsync();
    }

    public async Task<Booking?> GetByIdAsync(int id)
    {
        return await QueryWithIncludes().FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<Booking?> GetByIdForCustomerAsync(int id, int customerId)
    {
        return await QueryWithIncludes()
            .FirstOrDefaultAsync(b => b.Id == id && b.Vehicle != null && b.Vehicle.UserId == customerId);
    }

    public async Task<bool> IsTimeSlotTakenAsync(DateTime bookingDateTime, int serviceCatalogId)
    {
        return await _context.Bookings.AnyAsync(b =>
            b.BookingDateTime == bookingDateTime
            && b.ServiceCatalogId == serviceCatalogId
            && b.Status != BookingStatus.CANCELED);
    }

    public async Task<Booking> CreateAsync(Booking booking)
    {
        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();
        return booking;
    }

    public async Task UpdateAsync(Booking booking)
    {
        _context.Bookings.Update(booking);
        await _context.SaveChangesAsync();
    }

    private IQueryable<Booking> QueryWithIncludes()
    {
        return _context.Bookings
            .Include(b => b.Vehicle)
            .Include(b => b.ServiceCatalog)
            .Include(b => b.Mechanic)
            .Include(b => b.PaymentTransactions);
    }
}
