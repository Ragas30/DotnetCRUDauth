using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotnetCRUD.DTOs.Product;
using DotnetCRUD.Services;
using Microsoft.AspNetCore.Mvc;

namespace DotnetCRUD.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        [HttpPost("createProduct")]
        public async Task<IActionResult> CreateProduct(CreateProductDto dto)
        {
            try
            {
                var result = await _productService.CreateAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }
    }


}