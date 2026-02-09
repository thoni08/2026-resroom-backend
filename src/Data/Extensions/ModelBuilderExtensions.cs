using Microsoft.EntityFrameworkCore;
using ResRoomApi.Models;

namespace ResRoomApi.Data.Extensions;

public static class ModelBuilderExtensions
{
    public static void Seed(this ModelBuilder modelBuilder)
    {
        var seedTimestamp = new DateTime(2026, 2, 9, 0, 0, 0, DateTimeKind.Local);

        modelBuilder.Entity<Room>().HasData(
            new Room
            {
                Id = 1,
                Name = "Conference Room A",
                Capacity = 30,
                Location = "First Floor - East Building",
                Description = "A spacious conference room equipped with a projector and whiteboard.",
                CreatedAt = seedTimestamp,
                UpdatedAt = seedTimestamp
            },
            new Room
            {
                Id = 2,
                Name = "Meeting Room B",
                Capacity = 10,
                Location = "Second Floor - West Building",
                Description = "A cozy meeting room suitable for small team discussions.",
                CreatedAt = seedTimestamp,
                UpdatedAt = seedTimestamp
            }
        );
    }

    public static void ConfigureSoftDeleteQuery(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Room>()
            .HasQueryFilter(r => r.DeletedAt == null);

        modelBuilder.Entity<Reservation>()
            .HasQueryFilter(r => r.DeletedAt == null);
    }
}