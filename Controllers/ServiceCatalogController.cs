using DotnetCRUD.DTOs.ServiceCatalog;
using DotnetCRUD.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetCRUD.Controllers;

[ApiController]
[Route("api/services")]
[Authorize]
public class ServiceCatalogController : ControllerBase
{
    private readonly IServiceCatalogService _serviceCatalogService;
    private readonly IValidator<CreateServiceCatalogDto> _createValidator;
    private readonly IValidator<UpdateServiceCatalogDto> _updateValidator;

    public ServiceCatalogController(
        IServiceCatalogService serviceCatalogService,
        IValidator<CreateServiceCatalogDto> createValidator,
        IValidator<UpdateServiceCatalogDto> updateValidator)
    {
        _serviceCatalogService = serviceCatalogService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
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
        if (result == null)
        {
            return NotFound(new { message = "Layanan tidak ditemukan" });
        }

        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Create(CreateServiceCatalogDto dto)
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
            var result = await _serviceCatalogService.CreateAsync(dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Update(int id, UpdateServiceCatalogDto dto)
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
            var result = await _serviceCatalogService.UpdateAsync(id, dto);
            if (result == null)
            {
                return NotFound(new { message = "Layanan tidak ditemukan" });
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _serviceCatalogService.DeleteAsync(id);
        if (!deleted)
        {
            return NotFound(new { message = "Layanan tidak ditemukan" });
        }

        return Ok(new { message = "Layanan berhasil dihapus" });
    }
}
