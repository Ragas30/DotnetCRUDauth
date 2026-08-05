using DotnetCRUD.DTOs.Product;
using DotnetCRUD.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetCRUD.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpPost("createProduct")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> CreateProduct(CreateProductDto dto)
    {
        var result = await _productService.CreateAsync(dto);
        return Ok(result);
    }

    [HttpGet("getAllProducts")]
    public async Task<IActionResult> GetAllProducts()
    {
        var products = await _productService.GetAllAsync();
        return Ok(products);
    }

    [HttpGet("getProductById/{id}")]
    public async Task<IActionResult> GetProductById(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        return Ok(product);
    }

    [HttpPut("updateProduct/{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> UpdateProduct(int id, UpdateProductDto dto)
    {
        var result = await _productService.UpdateAsync(id, dto);
        return Ok(result);
    }

    [HttpDelete("deleteProduct/{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        await _productService.DeleteAsync(id);
        return Ok(new
        {
            message = "Product berhasil dihapus"
        });
    }
}
