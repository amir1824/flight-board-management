using FlightBoard.Api.Dal.DbModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FlightBoard.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> opt) : DbContext(opt)
{
    public DbSet<Flight> Flights => Set<Flight>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var utc = new ValueConverter<DateTime, DateTime>(
            v => v, 
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc)); // read

        modelBuilder.Entity<Flight>()
            .Property(f => f.DepartureTime)
            .HasConversion(utc);
    }
}
