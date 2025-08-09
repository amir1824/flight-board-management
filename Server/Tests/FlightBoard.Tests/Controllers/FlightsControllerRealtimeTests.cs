using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FlightBoard.Api.Controllers;
using FlightBoard.Api.Dal.DbModels;
using FlightBoard.Api.Dal.Interfaces;
using FlightBoard.Api.Features.Flights.Responses;
using FlightBoard.Api.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FlightBoard.Tests.Controllers;

public class FlightsControllerRealtimeTests
{
    private static (FlightsController ctrl, Mock<IFlightDal> dal, Mock<IUnitOfWork> uow, Mock<IClientProxy> proxy)
        BuildController()
    {
        var dal = new Mock<IFlightDal>(MockBehavior.Strict);
        var uow = new Mock<IUnitOfWork>(MockBehavior.Strict);

        var hub = new Mock<IHubContext<FlightsHub>>(MockBehavior.Strict);
        var clients = new Mock<IHubClients>(MockBehavior.Strict);
        var proxy = new Mock<IClientProxy>(MockBehavior.Strict);
        clients.Setup(c => c.All).Returns(proxy.Object);
        hub.SetupGet(h => h.Clients).Returns(clients.Object);

        var logger = new Mock<ILogger<CrudControllerBase<Flight>>>();
        var ctrl = new FlightsController(dal.Object, uow.Object, hub.Object, logger.Object);
        return (ctrl, dal, uow, proxy);
    }

    private static bool MatchFlightAdded(object[] args, Flight entity)
    {
        if (args.Length != 1) return false;

        var dto = args[0] as FlightResponse;
        if (dto == null) return false;

        if (dto.Id != entity.Id) return false;
        if (dto.FlightNumber != entity.FlightNumber) return false;
        if (dto.Destination != entity.Destination) return false;
        if (dto.Gate != entity.Gate) return false;

        return dto.Status == "Scheduled" ||
               dto.Status == "Boarding" ||
               dto.Status == "Departed";
    }

    private static bool MatchFlightUpdated(object[] args, int expectedId)
    {
        if (args.Length != 1) return false;

        var dto = args[0] as FlightResponse;
        if (dto == null) return false;

        return dto.Id == expectedId;
    }

    private static bool MatchFlightDeleted(object[] args, int expectedId)
    {
        return args.Length == 1 &&
               args[0] is int id &&
               id == expectedId;
    }

    [Fact]
    public async Task Create_Should_Publish_FlightAdded_With_ServerStatus()
    {
        var (ctrl, dal, uow, proxy) = BuildController();

        var entity = new Flight
        {
            Id = 1,
            FlightNumber = "LY001",
            Destination = "TLV",
            Gate = "A1",
            DepartureTime = DateTime.UtcNow.AddMinutes(45)
        };

        dal.Setup(d => d.AddAsync(entity, It.IsAny<CancellationToken>()))
           .Returns(Task.CompletedTask);
        uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
           .ReturnsAsync(1);

        proxy.Setup(p => p.SendCoreAsync(
                "FlightAdded",
                It.Is<object[]>(args => MatchFlightAdded(args, entity)),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

        var result = await ctrl.Create(entity, CancellationToken.None);
        result.Should().NotBeNull();

        proxy.Verify();
        dal.VerifyAll();
        uow.VerifyAll();
    }

    [Fact]
    public async Task Update_Should_Publish_FlightUpdated()
    {
        var (ctrl, dal, uow, proxy) = BuildController();

        var updated = new Flight
        {
            Id = 7,
            FlightNumber = "LH777",
            Destination = "FRA",
            Gate = "B2",
            DepartureTime = DateTime.UtcNow.AddMinutes(5)
        };

        dal.Setup(d => d.GetByIdAsync(7, It.IsAny<CancellationToken>()))
           .ReturnsAsync(new Flight { Id = 7 });
        dal.Setup(d => d.UpdateAsync(updated, It.IsAny<CancellationToken>()))
           .Returns(Task.CompletedTask);
        uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
           .ReturnsAsync(1);

        proxy.Setup(p => p.SendCoreAsync(
                "FlightUpdated",
                It.Is<object[]>(args => MatchFlightUpdated(args, updated.Id)),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

        await ctrl.Update(7, updated, CancellationToken.None);

        proxy.Verify();
        dal.VerifyAll();
        uow.VerifyAll();
    }

    [Fact]
    public async Task Delete_Should_Publish_FlightDeleted_With_Id()
    {
        var (ctrl, dal, uow, proxy) = BuildController();

        dal.Setup(d => d.GetByIdAsync(10, It.IsAny<CancellationToken>()))
           .ReturnsAsync(new Flight { Id = 10 });
        dal.Setup(d => d.RemoveAsync(It.IsAny<Flight>(), It.IsAny<CancellationToken>()))
           .Returns(Task.CompletedTask);
        uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
           .ReturnsAsync(1);

        proxy.Setup(p => p.SendCoreAsync(
                "FlightDeleted",
                It.Is<object[]>(args => MatchFlightDeleted(args, 10)),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

        await ctrl.Delete(10, CancellationToken.None);

        proxy.Verify();
        dal.VerifyAll();
        uow.VerifyAll();
    }
}
