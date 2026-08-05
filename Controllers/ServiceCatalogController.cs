using DotnetCRUD.DTOs.ServiceCatalog;
using DotnetCRUD.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetCRUD.Controllers;

[ApiController]
[Route("api/services")]
[Authorize]
public class ServiceCatalogController : ControllerBase
{
    private readonly IServiceCatalogService _serviceCatalogService;

    public ServiceCatalogController(IServiceCatalogService serviceCatalogService)
    {
        _serviceCatalogService = serviceCatalogService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll([FromQuery] bool activeOnly = false)
    {
        var result = await _serviceCatalogService.GetAllAsync(activeOnly);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _serviceCatalogService.GetByIdAsync(id);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Create(CreateServiceCatalogDto dto)
    {
        var result = await _serviceCatalogService.CreateAsync(dto);
        return Ok(result);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Update(int id, UpdateServiceCatalogDto dto)
    {
        var result = await _serviceCatalogService.UpdateAsync(id, dto);
        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Delete(int id)
    {
        await _serviceCatalogService.DeleteAsync(id);
        return Ok(new { message = "Layanan berhasil dihapus" });
    }
}
