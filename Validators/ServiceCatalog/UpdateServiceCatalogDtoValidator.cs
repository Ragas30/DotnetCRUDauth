using DotnetCRUD.DTOs.ServiceCatalog;
using FluentValidation;

namespace DotnetCRUD.Validators.ServiceCatalog;

public class UpdateServiceCatalogDtoValidator : AbstractValidator<UpdateServiceCatalogDto>
{
    public UpdateServiceCatalogDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nama layanan wajib diisi.")
            .MinimumLength(3).WithMessage("Nama layanan minimal 3 karakter.")
            .MaximumLength(120).WithMessage("Nama layanan maksimal 120 karakter.");

        RuleFor(x => x.DurationMinutes)
            .InclusiveBetween(15, 600).WithMessage("Durasi layanan harus 15-600 menit.");

        RuleFor(x => x.BasePrice)
            .GreaterThanOrEqualTo(0).WithMessage("Harga dasar tidak boleh negatif.");
    }
}
