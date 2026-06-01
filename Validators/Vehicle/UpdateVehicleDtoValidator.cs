using DotnetCRUD.DTOs.Vehicle;
using FluentValidation;

namespace DotnetCRUD.Validators.Vehicle;

public class UpdateVehicleDtoValidator : AbstractValidator<UpdateVehicleDto>
{
    public UpdateVehicleDtoValidator()
    {
        RuleFor(x => x.PlateNumber)
            .NotEmpty().WithMessage("Nomor plat wajib diisi.")
            .MaximumLength(20).WithMessage("Nomor plat maksimal 20 karakter.");

        RuleFor(x => x.Brand)
            .NotEmpty().WithMessage("Merek kendaraan wajib diisi.")
            .MaximumLength(100).WithMessage("Merek kendaraan maksimal 100 karakter.");

        RuleFor(x => x.Model)
            .NotEmpty().WithMessage("Model kendaraan wajib diisi.")
            .MaximumLength(100).WithMessage("Model kendaraan maksimal 100 karakter.");

        RuleFor(x => x.Year)
            .InclusiveBetween(1980, DateTime.UtcNow.Year + 1)
            .WithMessage("Tahun kendaraan tidak valid.");

        RuleFor(x => x.CurrentMileage)
            .GreaterThanOrEqualTo(0).WithMessage("Kilometer kendaraan tidak boleh negatif.");
    }
}
