using FlightBoard.Api.Dal.DbModels;
using FluentValidation;

public class FlightValidator : AbstractValidator<Flight>
{
    public FlightValidator()
    {
        RuleFor(f => f.FlightNumber)
            .NotEmpty().WithMessage("Flight number is required")
            .MaximumLength(10)
            .Matches("^[A-Za-z0-9]+$").WithMessage("Flight number can only contain letters and numbers");

        RuleFor(f => f.Destination)
            .NotEmpty().WithMessage("Destination is required")
            .MaximumLength(100);

        RuleFor(f => f.DepartureTime)
            .Must(dt => dt > DateTime.UtcNow)
            .WithMessage("Departure time must be in the future");

        RuleFor(f => f.Gate)
            .NotEmpty().WithMessage("Gate is required")
            .MaximumLength(5);
    }
}
