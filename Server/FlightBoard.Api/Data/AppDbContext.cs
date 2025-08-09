using FlightBoard.Api.Dal.DbModels;
using Microsoft.EntityFrameworkCore;

namespace FlightBoard.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> opt) : DbContext(opt)
{
    public DbSet<Flight> Flights => Set<Flight>();
    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Flight>().HasIndex(x => x.FlightNumber).IsUnique();
    }
}
