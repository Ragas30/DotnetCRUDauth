using DotnetCRUD.DTOs.Booking;
using FluentValidation;

namespace DotnetCRUD.Validators.Booking;

public class CreateBookingDtoValidator : AbstractValidator<CreateBookingDto>
{
    public CreateBookingDtoValidator()
    {
        RuleFor(x => x.VehicleId)
            .GreaterThan(0).WithMessage("VehicleId tidak valid.");

        RuleFor(x => x.ServiceCatalogId)
            .GreaterThan(0).WithMessage("ServiceCatalogId tidak valid.");

        RuleFor(x => x.BookingDateTime)
            .Must(date => date.ToUniversalTime() > DateTime.UtcNow)
            .WithMessage("Waktu booking harus di masa depan.");

        RuleFor(x => x.Complaint)
            .NotEmpty().WithMessage("Keluhan wajib diisi.")
            .MaximumLength(500).WithMessage("Keluhan maksimal 500 karakter.");
    }
}
