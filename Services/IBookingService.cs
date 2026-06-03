using DotnetCRUD.DTOs.Booking;

namespace DotnetCRUD.Services;

public interface IBookingService
{
    Task<List<BookingResponseDto>> GetBookingsAsync();
    Task<BookingResponseDto?> GetBookingByIdAsync(int id);
    Task<BookingResponseDto> CreateBookingAsync(CreateBookingDto dto);
    Task<BookingResponseDto?> UpdateStatusAsync(int bookingId, UpdateBookingStatusDto dto);
    Task<BookingResponseDto?> AssignMechanicAsync(int bookingId, AssignMechanicDto dto);
    Task<BookingResponseDto?> UpdateEstimateAsync(int bookingId, UpdateBookingEstimateDto dto);
    Task<BookingResponseDto?> UpdateServiceNotesAsync(int bookingId, UpdateBookingServiceNotesDto dto);
    Task<List<BookingHistoryResponseDto>> GetVehicleHistoryAsync(int vehicleId);
    Task<BookingResponseDto?> RecordManualPaymentAsync(int bookingId, ManualPaymentDto dto);
    Task<BookingInvoiceDto?> GetInvoiceAsync(int bookingId);
}
