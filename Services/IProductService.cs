using DotnetCRUD.DTOs.Product;

namespace DotnetCRUD.Services;

public interface IProductService
{
    Task<List<ProductResponseDto>> GetAllAsync();

    Task<ProductResponseDto?> GetByIdAsync(int id);

    Task<ProductResponseDto> CreateAsync(CreateProductDto createDto);

    Task<ProductResponseDto?> UpdateAsync(int id, UpdateProductDto updateDto);

    Task<bool> DeleteAsync(int id);
}