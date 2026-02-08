using Microsoft.EntityFrameworkCore;
using ResRoomApi.Models;

namespace ResRoomApi.Data;

public class ResRoomApiContext : DbContext
{
    public ResRoomApiContext(DbContextOptions<ResRoomApiContext> options) : base(options) { }

    public DbSet<Room> Rooms { get; set; }

    public DbSet<Reservation> Reservations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Room>()
            .HasQueryFilter(r => r.DeletedAt == null);

        modelBuilder.Entity<Reservation>()
            .HasQueryFilter(r => r.DeletedAt == null);
    }
}