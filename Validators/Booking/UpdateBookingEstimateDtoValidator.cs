using DotnetCRUD.DTOs.Booking;
using FluentValidation;

namespace DotnetCRUD.Validators.Booking;

public class UpdateBookingEstimateDtoValidator : AbstractValidator<UpdateBookingEstimateDto>
{
    public UpdateBookingEstimateDtoValidator()
    {
        RuleFor(x => x.EstimatedCost)
            .GreaterThanOrEqualTo(0).WithMessage("Estimasi biaya tidak boleh negatif.");
    }
}
