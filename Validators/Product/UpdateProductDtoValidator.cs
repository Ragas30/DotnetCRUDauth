using DotnetCRUD.DTOs.Product;
using FluentValidation;

namespace DotnetCRUD.Validators.Product;

public class UpdateProductDtoValidator : AbstractValidator<UpdateProductDto>
{
    public UpdateProductDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nama product wajib diisi.")
            .MinimumLength(3).WithMessage("Nama product minimal 3 karakter.")
            .MaximumLength(100).WithMessage("Nama product maksimal 100 karakter.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Deskripsi product maksimal 500 karakter.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Harga product harus lebih dari 0.");

        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0).WithMessage("Stock product tidak boleh negatif.");
    }
}
