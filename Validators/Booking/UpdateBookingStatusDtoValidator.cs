using DotnetCRUD.DTOs.Booking;
using FluentValidation;

namespace DotnetCRUD.Validators.Booking;

public class UpdateBookingStatusDtoValidator : AbstractValidator<UpdateBookingStatusDto>
{
    public UpdateBookingStatusDtoValidator()
    {
        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Status booking tidak valid.");
    }
}
