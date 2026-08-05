using DotnetCRUD.DTOs.Vehicle;
using DotnetCRUD.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetCRUD.Controllers;

[ApiController]
[Route("api/vehicles")]
[Authorize(Roles = "CUSTOMER")]
public class VehicleController : ControllerBase
{
    private readonly IVehicleService _vehicleService;

    public VehicleController(IVehicleService vehicleService)
    {
        _vehicleService = vehicleService;
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
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateVehicleDto dto)
    {
        var result = await _vehicleService.CreateAsync(dto);
        return Ok(result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateVehicleDto dto)
    {
        var result = await _vehicleService.UpdateAsync(id, dto);
        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _vehicleService.DeleteAsync(id);
        return Ok(new { message = "Kendaraan berhasil dihapus" });
    }
}
