using DotnetCRUD.DTOs.Booking;
using FluentValidation;

namespace DotnetCRUD.Validators.Booking;

public class AssignMechanicDtoValidator : AbstractValidator<AssignMechanicDto>
{
    public AssignMechanicDtoValidator()
    {
        RuleFor(x => x.MechanicId)
            .GreaterThan(0).WithMessage("MechanicId tidak valid.");
    }
}
