using DotnetCRUD.DTOs.Booking;
using DotnetCRUD.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetCRUD.Controllers;

[ApiController]
[Route("api/bookings")]
[Authorize]
public class BookingController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    [HttpGet]
    public async Task<IActionResult> GetBookings()
    {
        var result = await _bookingService.GetBookingsAsync();
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetBookingById(int id)
    {
        var result = await _bookingService.GetBookingByIdAsync(id);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "CUSTOMER")]
    public async Task<IActionResult> CreateBooking(CreateBookingDto dto)
    {
        var result = await _bookingService.CreateBookingAsync(dto);
        return Ok(result);
    }

    [HttpPut("{id:int}/status")]
    [Authorize(Roles = "ADMIN,MECHANIC")]
    public async Task<IActionResult> UpdateStatus(int id, UpdateBookingStatusDto dto)
    {
        var result = await _bookingService.UpdateStatusAsync(id, dto);
        return Ok(result);
    }

    [HttpPut("{id:int}/assign-mechanic")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> AssignMechanic(int id, AssignMechanicDto dto)
    {
        var result = await _bookingService.AssignMechanicAsync(id, dto);
        return Ok(result);
    }

    [HttpPut("{id:int}/estimate")]
    [Authorize(Roles = "ADMIN,MECHANIC")]
    public async Task<IActionResult> UpdateEstimate(int id, UpdateBookingEstimateDto dto)
    {
        var result = await _bookingService.UpdateEstimateAsync(id, dto);
        return Ok(result);
    }

    [HttpPut("{id:int}/service-notes")]
    [Authorize(Roles = "ADMIN,MECHANIC")]
    public async Task<IActionResult> UpdateServiceNotes(int id, UpdateBookingServiceNotesDto dto)
    {
        var result = await _bookingService.UpdateServiceNotesAsync(id, dto);
        return Ok(result);
    }

    [HttpGet("/api/vehicles/{vehicleId:int}/service-history")]
    public async Task<IActionResult> GetVehicleHistory(int vehicleId)
    {
        var result = await _bookingService.GetVehicleHistoryAsync(vehicleId);
        return Ok(result);
    }

    [HttpPut("{id:int}/payment/manual")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> RecordManualPayment(int id, ManualPaymentDto dto)
    {
        var result = await _bookingService.RecordManualPaymentAsync(id, dto);
        return Ok(result);
    }

    [HttpGet("{id:int}/invoice")]
    public async Task<IActionResult> GetInvoice(int id)
    {
        var result = await _bookingService.GetInvoiceAsync(id);
        return Ok(result);
    }
}
