using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FlightBoard.Api.Dal.DbModels;
using FlightBoard.Api.Dal.Ef;
using FlightBoard.Api.Data;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FlightBoard.Tests.Dal;

public class FlightEfDalTests
{
    private static AppDbContext BuildDb(string name)
    {
        var opts = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(name)
            .Options;
        return new AppDbContext(opts);
    }

    [Fact]
    public async Task ExistsByNumberAsync_Should_Work()
    {
        await using var db = BuildDb(nameof(ExistsByNumberAsync_Should_Work));
        db.Flights.Add(new Flight { Id=1, FlightNumber="LY001", Destination="TLV", Gate="A1", DepartureTime=System.DateTime.UtcNow.AddHours(1) });
        await db.SaveChangesAsync();

        var dal = new FlightEfDal(db);
        (await dal.ExistsByNumberAsync("LY001", CancellationToken.None)).Should().BeTrue();
        (await dal.ExistsByNumberAsync("XXX",    CancellationToken.None)).Should().BeFalse();
    }

    [Fact]
    public async Task SearchAsync_Should_Filter_By_Destination_Substring()
    {
        await using var db = BuildDb(nameof(SearchAsync_Should_Filter_By_Destination_Substring));
        db.Flights.AddRange(
            new Flight { Id=1, FlightNumber="A1", Destination="Tel Aviv", Gate="A", DepartureTime=System.DateTime.UtcNow.AddHours(1) },
            new Flight { Id=2, FlightNumber="B1", Destination="Paris",    Gate="B", DepartureTime=System.DateTime.UtcNow.AddHours(2) }
        );
        await db.SaveChangesAsync();

        var dal = new FlightEfDal(db);

        var r1 = await dal.SearchAsync("Tel", CancellationToken.None);
        r1.Should().HaveCount(1);
        r1.Single().Destination.Should().Contain("Tel");

        var r2 = await dal.SearchAsync("aris", CancellationToken.None);
        r2.Should().HaveCount(1);
        r2.Single().Destination.Should().Contain("aris");

        var r3 = await dal.SearchAsync("NYC", CancellationToken.None);
        r3.Should().BeEmpty();
    }
}
