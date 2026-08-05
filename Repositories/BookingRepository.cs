using DotnetCRUD.Data;
using DotnetCRUD.Exceptions;
using DotnetCRUD.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

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

    public async Task<Booking> CreateWithSlotGuardAsync(Booking booking, int durationMinutes)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);

        var slotTaken = await IsTimeSlotTakenAsync(booking.BookingDateTime, durationMinutes);
        if (slotTaken)
        {
            throw new ConflictException("BOOKING_SLOT_TAKEN", "Slot booking pada waktu tersebut sudah terisi");
        }

        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();
        await transaction.CommitAsync();
        return booking;
    }

    public async Task UpdateAsync(Booking booking)
    {
        booking.Version++;
        _context.Bookings.Update(booking);
        await _context.SaveChangesAsync();
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return await _context.Database.BeginTransactionAsync();
    }

    private async Task<bool> IsTimeSlotTakenAsync(DateTime startUtc, int durationMinutes)
    {
        var endUtc = startUtc.AddMinutes(durationMinutes);

        return await (
            from b in _context.Bookings
            join s in _context.ServiceCatalogs on b.ServiceCatalogId equals s.Id
            where b.Status != BookingStatus.CANCELED
                  && b.BookingDateTime < endUtc
                  && startUtc < b.BookingDateTime.AddMinutes(s.DurationMinutes)
            select b.Id)
            .AnyAsync();
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
