using DotnetCRUD.DTOs.Booking;
using DotnetCRUD.Models;
using FluentValidation;

namespace DotnetCRUD.Validators.Booking;

public class ManualPaymentDtoValidator : AbstractValidator<ManualPaymentDto>
{
    public ManualPaymentDtoValidator()
    {
        RuleFor(x => x.PaymentMethod)
            .Must(method => method != PaymentMethod.GATEWAY)
            .WithMessage("Manual payment tidak boleh menggunakan metode gateway.");

        RuleFor(x => x.PaidAmount)
            .GreaterThan(0).WithMessage("Jumlah pembayaran harus lebih dari 0.");

        RuleFor(x => x.ReferenceNumber)
            .MaximumLength(100).WithMessage("Nomor referensi maksimal 100 karakter.");
    }
}
