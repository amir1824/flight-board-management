using FluentValidation;
using FlightBoard.Api.Dal.DbModels;
using FlightBoard.Api.Dal.Interfaces;

namespace FlightBoard.Api.Features.Flights.Validation;

public class FlightValidator : AbstractValidator<Flight>
{
    public FlightValidator(IFlightDal dal)
    {
        RuleFor(x => x.FlightNumber)
            .NotEmpty().WithMessage("Flight number is required.")
            .MaximumLength(32);

        RuleFor(x => x.Destination)
            .NotEmpty().WithMessage("Destination is required.")
            .MaximumLength(64);

        RuleFor(x => x.DepartureTime)
            .NotEmpty().WithMessage("Departure time is required.");

        RuleFor(x => x)
            .MustAsync(async (f, ct) =>
                !await dal.ExistsAsync(x =>
                    x.FlightNumber == f.FlightNumber &&
                    x.Id != f.Id, ct))
            .WithMessage("Flight number already exists.")
            .WithErrorCode("Conflict");
    }
}