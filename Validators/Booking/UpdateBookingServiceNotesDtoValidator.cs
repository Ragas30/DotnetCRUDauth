using DotnetCRUD.DTOs.Booking;
using FluentValidation;

namespace DotnetCRUD.Validators.Booking;

public class UpdateBookingServiceNotesDtoValidator : AbstractValidator<UpdateBookingServiceNotesDto>
{
    public UpdateBookingServiceNotesDtoValidator()
    {
        RuleFor(x => x.ServiceNotes)
            .NotEmpty().WithMessage("Catatan servis wajib diisi.")
            .MaximumLength(2000).WithMessage("Catatan servis maksimal 2000 karakter.");

        RuleFor(x => x.RecommendedNextServiceMileage)
            .GreaterThan(0)
            .When(x => x.RecommendedNextServiceMileage.HasValue)
            .WithMessage("Rekomendasi kilometer servis berikutnya harus lebih dari 0.");
    }
}
