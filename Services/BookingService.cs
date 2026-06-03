using DotnetCRUD.DTOs.Booking;
using DotnetCRUD.Models;
using DotnetCRUD.Repositories;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DotnetCRUD.Services;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IPaymentTransactionRepository _paymentTransactionRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IServiceCatalogRepository _serviceCatalogRepository;
    private readonly IUserRepository _userRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public BookingService(
        IBookingRepository bookingRepository,
        IPaymentTransactionRepository paymentTransactionRepository,
        IVehicleRepository vehicleRepository,
        IServiceCatalogRepository serviceCatalogRepository,
        IUserRepository userRepository,
        IHttpContextAccessor httpContextAccessor)
    {
        _bookingRepository = bookingRepository;
        _paymentTransactionRepository = paymentTransactionRepository;
        _vehicleRepository = vehicleRepository;
        _serviceCatalogRepository = serviceCatalogRepository;
        _userRepository = userRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<List<BookingResponseDto>> GetBookingsAsync()
    {
        var role = GetCurrentUserRole();
        var userId = GetCurrentUserId();

        List<Booking> bookings;

        if (role == UserRole.ADMIN)
        {
            bookings = await _bookingRepository.GetAllAsync();
        }
        else if (role == UserRole.MECHANIC)
        {
            bookings = await _bookingRepository.GetByMechanicIdAsync(userId);
        }
        else
        {
            bookings = await _bookingRepository.GetByCustomerIdAsync(userId);
        }

        return bookings.Select(MapToResponse).ToList();
    }

    public async Task<BookingResponseDto?> GetBookingByIdAsync(int id)
    {
        var role = GetCurrentUserRole();
        var userId = GetCurrentUserId();

        Booking? booking;

        if (role == UserRole.ADMIN)
        {
            booking = await _bookingRepository.GetByIdAsync(id);
        }
        else if (role == UserRole.MECHANIC)
        {
            booking = await _bookingRepository.GetByIdAsync(id);
            if (booking?.MechanicId != userId)
            {
                return null;
            }
        }
        else
        {
            booking = await _bookingRepository.GetByIdForCustomerAsync(id, userId);
        }

        return booking == null ? null : MapToResponse(booking);
    }

    public async Task<BookingResponseDto> CreateBookingAsync(CreateBookingDto dto)
    {
        var userId = GetCurrentUserId();

        var vehicle = await _vehicleRepository.GetByIdAndUserIdAsync(dto.VehicleId, userId);
        if (vehicle == null)
        {
            throw new Exception("Kendaraan tidak ditemukan atau bukan milik Anda");
        }

        var serviceCatalog = await _serviceCatalogRepository.GetByIdAsync(dto.ServiceCatalogId);
        if (serviceCatalog == null || !serviceCatalog.IsActive)
        {
            throw new Exception("Layanan tidak ditemukan atau tidak aktif");
        }

        var bookingTimeUtc = dto.BookingDateTime.ToUniversalTime();
        var slotTaken = await _bookingRepository.IsTimeSlotTakenAsync(bookingTimeUtc, dto.ServiceCatalogId);
        if (slotTaken)
        {
            throw new Exception("Slot booking pada waktu tersebut sudah terisi");
        }

        var booking = new Booking
        {
            VehicleId = dto.VehicleId,
            ServiceCatalogId = dto.ServiceCatalogId,
            BookingDateTime = bookingTimeUtc,
            Complaint = dto.Complaint.Trim(),
            Status = BookingStatus.BOOKED,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId.ToString()
        };

        await _bookingRepository.CreateAsync(booking);
        var created = await _bookingRepository.GetByIdAsync(booking.Id);

        if (created == null)
        {
            throw new Exception("Gagal mengambil data booking setelah dibuat");
        }

        return MapToResponse(created);
    }

    public async Task<BookingResponseDto?> UpdateStatusAsync(int bookingId, UpdateBookingStatusDto dto)
    {
        var userId = GetCurrentUserId();
        var role = GetCurrentUserRole();

        var booking = await _bookingRepository.GetByIdAsync(bookingId);
        if (booking == null)
        {
            return null;
        }

        if (role == UserRole.MECHANIC && booking.MechanicId != userId)
        {
            throw new Exception("Anda hanya bisa update booking yang di-assign ke Anda");
        }

        if (role == UserRole.CUSTOMER)
        {
            throw new Exception("Customer tidak diizinkan mengubah status booking");
        }

        ValidateStatusTransition(booking.Status, dto.Status);

        booking.Status = dto.Status;
        if (dto.Status == BookingStatus.DONE)
        {
            booking.CompletedAt = DateTime.UtcNow;
        }
        booking.UpdatedAt = DateTime.UtcNow;
        booking.UpdatedBy = userId.ToString();

        await _bookingRepository.UpdateAsync(booking);
        var updated = await _bookingRepository.GetByIdAsync(bookingId);

        return updated == null ? null : MapToResponse(updated);
    }

    public async Task<BookingResponseDto?> UpdateEstimateAsync(int bookingId, UpdateBookingEstimateDto dto)
    {
        var role = GetCurrentUserRole();
        var userId = GetCurrentUserId();

        if (role != UserRole.ADMIN && role != UserRole.MECHANIC)
        {
            throw new Exception("Anda tidak diizinkan mengubah estimasi biaya");
        }

        var booking = await _bookingRepository.GetByIdAsync(bookingId);
        if (booking == null)
        {
            return null;
        }

        if (role == UserRole.MECHANIC && booking.MechanicId != userId)
        {
            throw new Exception("Anda hanya bisa update booking yang di-assign ke Anda");
        }

        if (booking.Status == BookingStatus.CANCELED || booking.Status == BookingStatus.PAID)
        {
            throw new Exception("Estimasi biaya tidak bisa diubah pada status booking saat ini");
        }

        booking.EstimatedCost = dto.EstimatedCost;
        booking.UpdatedAt = DateTime.UtcNow;
        booking.UpdatedBy = userId.ToString();

        await _bookingRepository.UpdateAsync(booking);
        var updated = await _bookingRepository.GetByIdAsync(bookingId);

        return updated == null ? null : MapToResponse(updated);
    }

    public async Task<BookingResponseDto?> UpdateServiceNotesAsync(int bookingId, UpdateBookingServiceNotesDto dto)
    {
        var role = GetCurrentUserRole();
        var userId = GetCurrentUserId();

        if (role != UserRole.ADMIN && role != UserRole.MECHANIC)
        {
            throw new Exception("Anda tidak diizinkan mengubah catatan servis");
        }

        var booking = await _bookingRepository.GetByIdAsync(bookingId);
        if (booking == null)
        {
            return null;
        }

        if (role == UserRole.MECHANIC && booking.MechanicId != userId)
        {
            throw new Exception("Anda hanya bisa update booking yang di-assign ke Anda");
        }

        if (booking.Status != BookingStatus.INSERVICE && booking.Status != BookingStatus.DONE)
        {
            throw new Exception("Catatan servis hanya bisa diisi saat kendaraan sedang atau sudah selesai diservis");
        }

        booking.ServiceNotes = dto.ServiceNotes.Trim();
        booking.RecommendedNextServiceDate = dto.RecommendedNextServiceDate?.ToUniversalTime();
        booking.RecommendedNextServiceMileage = dto.RecommendedNextServiceMileage;
        booking.UpdatedAt = DateTime.UtcNow;
        booking.UpdatedBy = userId.ToString();

        await _bookingRepository.UpdateAsync(booking);
        var updated = await _bookingRepository.GetByIdAsync(bookingId);

        return updated == null ? null : MapToResponse(updated);
    }

    public async Task<List<BookingHistoryResponseDto>> GetVehicleHistoryAsync(int vehicleId)
    {
        var role = GetCurrentUserRole();
        var userId = GetCurrentUserId();

        if (role == UserRole.CUSTOMER)
        {
            var vehicle = await _vehicleRepository.GetByIdAndUserIdAsync(vehicleId, userId);
            if (vehicle == null)
            {
                throw new Exception("Kendaraan tidak ditemukan atau bukan milik Anda");
            }
        }
        else
        {
            var vehicle = await _vehicleRepository.GetByIdAsync(vehicleId);
            if (vehicle == null)
            {
                throw new Exception("Kendaraan tidak ditemukan");
            }
        }

        var history = await _bookingRepository.GetHistoryByVehicleIdAsync(vehicleId);

        return history.Select(booking => new BookingHistoryResponseDto
        {
            BookingId = booking.Id,
            VehicleId = booking.VehicleId,
            PlateNumber = booking.Vehicle?.PlateNumber ?? string.Empty,
            ServiceName = booking.ServiceCatalog?.Name ?? string.Empty,
            BookingDateTime = booking.BookingDateTime,
            Status = booking.Status,
            EstimatedCost = booking.EstimatedCost,
            ServiceNotes = booking.ServiceNotes,
            CompletedAt = booking.CompletedAt,
            RecommendedNextServiceDate = booking.RecommendedNextServiceDate,
            RecommendedNextServiceMileage = booking.RecommendedNextServiceMileage
        }).ToList();
    }

    public async Task<BookingResponseDto?> RecordManualPaymentAsync(int bookingId, ManualPaymentDto dto)
    {
        var role = GetCurrentUserRole();
        var userId = GetCurrentUserId();

        if (role != UserRole.ADMIN)
        {
            throw new Exception("Hanya admin yang dapat memproses pembayaran manual");
        }

        var booking = await _bookingRepository.GetByIdAsync(bookingId);
        if (booking == null)
        {
            return null;
        }

        if (booking.Status != BookingStatus.DONE)
        {
            throw new Exception("Pembayaran manual hanya bisa diproses setelah servis selesai");
        }

        var expectedAmount = booking.EstimatedCost ?? 0m;
        if (expectedAmount <= 0)
        {
            throw new Exception("Estimasi biaya belum tersedia");
        }

        if (dto.PaidAmount < expectedAmount)
        {
            throw new Exception("Jumlah pembayaran kurang dari estimasi biaya");
        }

        var paymentTransaction = new PaymentTransaction
        {
            BookingId = booking.Id,
            Provider = "MANUAL",
            PaymentMethod = dto.PaymentMethod,
            PaymentStatus = PaymentStatus.PAID,
            Amount = dto.PaidAmount,
            ReferenceNumber = string.IsNullOrWhiteSpace(dto.ReferenceNumber) ? null : dto.ReferenceNumber.Trim(),
            PaidAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId.ToString()
        };

        await _paymentTransactionRepository.CreateAsync(paymentTransaction);

        booking.PaymentStatus = PaymentStatus.PAID;
        booking.Status = BookingStatus.PAID;
        booking.UpdatedAt = DateTime.UtcNow;
        booking.UpdatedBy = userId.ToString();

        await _bookingRepository.UpdateAsync(booking);
        var updated = await _bookingRepository.GetByIdAsync(bookingId);

        return updated == null ? null : MapToResponse(updated);
    }

    public async Task<BookingInvoiceDto?> GetInvoiceAsync(int bookingId)
    {
        var role = GetCurrentUserRole();
        var userId = GetCurrentUserId();

        Booking? booking;

        if (role == UserRole.ADMIN)
        {
            booking = await _bookingRepository.GetByIdAsync(bookingId);
        }
        else if (role == UserRole.CUSTOMER)
        {
            booking = await _bookingRepository.GetByIdForCustomerAsync(bookingId, userId);
        }
        else if (role == UserRole.MECHANIC)
        {
            booking = await _bookingRepository.GetByIdAsync(bookingId);
            if (booking?.MechanicId != userId)
            {
                return null;
            }
        }
        else
        {
            booking = null;
        }

        if (booking == null)
        {
            return null;
        }

        var latestPayment = booking.PaymentTransactions
            .OrderByDescending(payment => payment.CreatedAt)
            .FirstOrDefault();

        return new BookingInvoiceDto
        {
            BookingId = booking.Id,
            PlateNumber = booking.Vehicle?.PlateNumber ?? string.Empty,
            ServiceName = booking.ServiceCatalog?.Name ?? string.Empty,
            BookingDateTime = booking.BookingDateTime,
            CompletedAt = booking.CompletedAt,
            EstimatedCost = booking.EstimatedCost ?? 0m,
            PaymentStatus = booking.PaymentStatus,
            PaymentMethod = latestPayment?.PaymentMethod,
            PaidAmount = latestPayment?.Amount,
            PaidAt = latestPayment?.PaidAt,
            ReferenceNumber = latestPayment?.ReferenceNumber,
            ServiceNotes = booking.ServiceNotes
        };
    }

    public async Task<BookingResponseDto?> AssignMechanicAsync(int bookingId, AssignMechanicDto dto)
    {
        var role = GetCurrentUserRole();
        var userId = GetCurrentUserId();

        if (role != UserRole.ADMIN)
        {
            throw new Exception("Hanya admin yang dapat assign mekanik");
        }

        var booking = await _bookingRepository.GetByIdAsync(bookingId);
        if (booking == null)
        {
            return null;
        }

        var mechanic = await _userRepository.GetByIdAsync(dto.MechanicId);
        if (mechanic == null || mechanic.Role != UserRole.MECHANIC)
        {
            throw new Exception("Mechanic tidak ditemukan");
        }

        booking.MechanicId = dto.MechanicId;
        booking.UpdatedAt = DateTime.UtcNow;
        booking.UpdatedBy = userId.ToString();

        await _bookingRepository.UpdateAsync(booking);
        var updated = await _bookingRepository.GetByIdAsync(bookingId);

        return updated == null ? null : MapToResponse(updated);
    }

    private static void ValidateStatusTransition(BookingStatus current, BookingStatus next)
    {
        var allowed = new Dictionary<BookingStatus, BookingStatus[]>
        {
            { BookingStatus.BOOKED, new[] { BookingStatus.CHECKIN, BookingStatus.CANCELED } },
            { BookingStatus.CHECKIN, new[] { BookingStatus.INSERVICE, BookingStatus.CANCELED } },
            { BookingStatus.INSERVICE, new[] { BookingStatus.DONE } },
            { BookingStatus.DONE, new[] { BookingStatus.PAID } },
            { BookingStatus.PAID, Array.Empty<BookingStatus>() },
            { BookingStatus.CANCELED, Array.Empty<BookingStatus>() }
        };

        if (!allowed[current].Contains(next))
        {
            throw new Exception($"Transisi status tidak valid: {current} -> {next}");
        }
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

    private UserRole GetCurrentUserRole()
    {
        var roleValue = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Role);
        if (!Enum.TryParse<UserRole>(roleValue, out var role))
        {
            throw new Exception("Role user tidak valid");
        }

        return role;
    }

    private static BookingResponseDto MapToResponse(Booking booking)
    {
        return new BookingResponseDto
        {
            Id = booking.Id,
            VehicleId = booking.VehicleId,
            PlateNumber = booking.Vehicle?.PlateNumber ?? string.Empty,
            ServiceCatalogId = booking.ServiceCatalogId,
            ServiceName = booking.ServiceCatalog?.Name ?? string.Empty,
            BookingDateTime = booking.BookingDateTime,
            Complaint = booking.Complaint,
            Status = booking.Status,
            EstimatedCost = booking.EstimatedCost,
            PaymentStatus = booking.PaymentStatus,
            ServiceNotes = booking.ServiceNotes,
            CompletedAt = booking.CompletedAt,
            RecommendedNextServiceDate = booking.RecommendedNextServiceDate,
            RecommendedNextServiceMileage = booking.RecommendedNextServiceMileage,
            MechanicId = booking.MechanicId
        };
    }
}
