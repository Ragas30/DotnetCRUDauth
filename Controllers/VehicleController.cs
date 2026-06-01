using DotnetCRUD.DTOs.Vehicle;
using DotnetCRUD.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetCRUD.Controllers;

[ApiController]
[Route("api/vehicles")]
[Authorize(Roles = "CUSTOMER")]
public class VehicleController : ControllerBase
{
    private readonly IVehicleService _vehicleService;
    private readonly IValidator<CreateVehicleDto> _createValidator;
    private readonly IValidator<UpdateVehicleDto> _updateValidator;

    public VehicleController(
        IVehicleService vehicleService,
        IValidator<CreateVehicleDto> createValidator,
        IValidator<UpdateVehicleDto> updateValidator)
    {
        _vehicleService = vehicleService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyVehicles()
    {
        var result = await _vehicleService.GetMyVehiclesAsync();
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetMyVehicleById(int id)
    {
        var result = await _vehicleService.GetMyVehicleByIdAsync(id);
        if (result == null)
        {
            return NotFound(new { message = "Kendaraan tidak ditemukan" });
        }

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateVehicleDto dto)
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
            var result = await _vehicleService.CreateAsync(dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateVehicleDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
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
            var result = await _vehicleService.UpdateAsync(id, dto);
            if (result == null)
            {
                return NotFound(new { message = "Kendaraan tidak ditemukan" });
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _vehicleService.DeleteAsync(id);
        if (!deleted)
        {
            return NotFound(new { message = "Kendaraan tidak ditemukan" });
        }

        return Ok(new { message = "Kendaraan berhasil dihapus" });
    }
}
