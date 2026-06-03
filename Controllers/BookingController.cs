using DotnetCRUD.DTOs.Booking;
using DotnetCRUD.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetCRUD.Controllers;

[ApiController]
[Route("api/bookings")]
[Authorize]
public class BookingController : ControllerBase
{
    private readonly IBookingService _bookingService;
    private readonly IValidator<CreateBookingDto> _createValidator;
    private readonly IValidator<UpdateBookingStatusDto> _statusValidator;
    private readonly IValidator<AssignMechanicDto> _assignValidator;
    private readonly IValidator<UpdateBookingEstimateDto> _estimateValidator;
    private readonly IValidator<UpdateBookingServiceNotesDto> _serviceNotesValidator;
    private readonly IValidator<ManualPaymentDto> _manualPaymentValidator;

    public BookingController(
        IBookingService bookingService,
        IValidator<CreateBookingDto> createValidator,
        IValidator<UpdateBookingStatusDto> statusValidator,
        IValidator<AssignMechanicDto> assignValidator,
        IValidator<UpdateBookingEstimateDto> estimateValidator,
        IValidator<UpdateBookingServiceNotesDto> serviceNotesValidator,
        IValidator<ManualPaymentDto> manualPaymentValidator)
    {
        _bookingService = bookingService;
        _createValidator = createValidator;
        _statusValidator = statusValidator;
        _assignValidator = assignValidator;
        _estimateValidator = estimateValidator;
        _serviceNotesValidator = serviceNotesValidator;
        _manualPaymentValidator = manualPaymentValidator;
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
        if (result == null)
        {
            return NotFound(new { message = "Booking tidak ditemukan" });
        }

        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "CUSTOMER")]
    public async Task<IActionResult> CreateBooking(CreateBookingDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            return BadRequest(new
            {
                errors = validationResult.Errors.Select(error => new
                {
                    field = error.PropertyName,
                    message = error.ErrorMessage
                })
            });
        }

        try
        {
            var result = await _bookingService.CreateBookingAsync(dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:int}/status")]
    [Authorize(Roles = "ADMIN,MECHANIC")]
    public async Task<IActionResult> UpdateStatus(int id, UpdateBookingStatusDto dto)
    {
        var validationResult = await _statusValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            return BadRequest(new
            {
                errors = validationResult.Errors.Select(error => new
                {
                    field = error.PropertyName,
                    message = error.ErrorMessage
                })
            });
        }

        try
        {
            var result = await _bookingService.UpdateStatusAsync(id, dto);
            if (result == null)
            {
                return NotFound(new { message = "Booking tidak ditemukan" });
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:int}/assign-mechanic")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> AssignMechanic(int id, AssignMechanicDto dto)
    {
        var validationResult = await _assignValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            return BadRequest(new
            {
                errors = validationResult.Errors.Select(error => new
                {
                    field = error.PropertyName,
                    message = error.ErrorMessage
                })
            });
        }

        try
        {
            var result = await _bookingService.AssignMechanicAsync(id, dto);
            if (result == null)
            {
                return NotFound(new { message = "Booking tidak ditemukan" });
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:int}/estimate")]
    [Authorize(Roles = "ADMIN,MECHANIC")]
    public async Task<IActionResult> UpdateEstimate(int id, UpdateBookingEstimateDto dto)
    {
        var validationResult = await _estimateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            return BadRequest(new
            {
                errors = validationResult.Errors.Select(error => new
                {
                    field = error.PropertyName,
                    message = error.ErrorMessage
                })
            });
        }

        try
        {
            var result = await _bookingService.UpdateEstimateAsync(id, dto);
            if (result == null)
            {
                return NotFound(new { message = "Booking tidak ditemukan" });
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:int}/service-notes")]
    [Authorize(Roles = "ADMIN,MECHANIC")]
    public async Task<IActionResult> UpdateServiceNotes(int id, UpdateBookingServiceNotesDto dto)
    {
        var validationResult = await _serviceNotesValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            return BadRequest(new
            {
                errors = validationResult.Errors.Select(error => new
                {
                    field = error.PropertyName,
                    message = error.ErrorMessage
                })
            });
        }

        try
        {
            var result = await _bookingService.UpdateServiceNotesAsync(id, dto);
            if (result == null)
            {
                return NotFound(new { message = "Booking tidak ditemukan" });
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("/api/vehicles/{vehicleId:int}/service-history")]
    public async Task<IActionResult> GetVehicleHistory(int vehicleId)
    {
        try
        {
            var result = await _bookingService.GetVehicleHistoryAsync(vehicleId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:int}/payment/manual")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> RecordManualPayment(int id, ManualPaymentDto dto)
    {
        var validationResult = await _manualPaymentValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            return BadRequest(new
            {
                errors = validationResult.Errors.Select(error => new
                {
                    field = error.PropertyName,
                    message = error.ErrorMessage
                })
            });
        }

        try
        {
            var result = await _bookingService.RecordManualPaymentAsync(id, dto);
            if (result == null)
            {
                return NotFound(new { message = "Booking tidak ditemukan" });
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id:int}/invoice")]
    public async Task<IActionResult> GetInvoice(int id)
    {
        var result = await _bookingService.GetInvoiceAsync(id);
        if (result == null)
        {
            return NotFound(new { message = "Invoice tidak ditemukan" });
        }

        return Ok(result);
    }
}
