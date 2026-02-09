using Microsoft.EntityFrameworkCore;
using ResRoomApi.Models;

namespace ResRoomApi.Data.Extensions;

public static class ModelBuilderExtensions
{
    public static void Seed(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Room>().HasData(
            new Room
            {
                Id = 1,
                Name = "Conference Room A",
                Capacity = 30,
                Location = "First Floor - East Building",
                Description = "A spacious conference room equipped with a projector and whiteboard.",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            },
            new Room
            {
                Id = 2,
                Name = "Meeting Room B",
                Capacity = 10,
                Location = "Second Floor - West Building",
                Description = "A cozy meeting room suitable for small team discussions.",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            }
        );

        modelBuilder.Entity<Reservation>().HasData(
            new Reservation
            {
                Id = 1,
                RoomId = 2,
                StartTime = DateTime.Now.AddDays(1).AddHours(9),
                EndTime = DateTime.Now.AddDays(1).AddHours(11),
                ReservedBy = "Alice Johnson",
                Purpose = "Project Kickoff Meeting",
                Status = ReservationStatus.Approved,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            }
        );
    }
}