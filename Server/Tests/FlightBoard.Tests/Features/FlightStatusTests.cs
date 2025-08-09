using System;
using FluentAssertions;
using FlightBoard.Api.Features.Flights;
using Xunit;

namespace FlightBoard.Tests.Features;

public class FlightStatusTests
{
    [Fact]
    public void FutureFlight_ShouldBe_Scheduled()
    {
        var now = DateTime.UtcNow;
        var dep = now.AddMinutes(45);
        FlightStatus.From(dep, now).Should().Be("Scheduled");
    }

    [Fact]
    public void AtDeparture_ShouldBe_Boarding_Or_Departed()
    {
        var now = DateTime.UtcNow;
        var dep = now;
        var status = FlightStatus.From(dep, now);
        status.Should().BeOneOf("Boarding", "Departed");
    }

    [Fact]
    public void PastFlight_ShouldBe_Departed()
    {
        var now = DateTime.UtcNow;
        var dep = now.AddMinutes(-10);
        FlightStatus.From(dep, now).Should().Be("Departed");
    }
}
