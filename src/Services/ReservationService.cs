using Microsoft.EntityFrameworkCore;
using ResRoomApi.Data;
using ResRoomApi.Models;

namespace ResRoomApi.Services;

public class ReservationService : IReservationService
{
    private readonly ResRoomApiContext _context;

    public ReservationService(ResRoomApiContext context)
    {
        _context = context;
    }

    public async Task<bool> IsRoomAvailableAsync(int roomId, DateTime start, DateTime end)
    {
        return !await _context.Reservations
            .AnyAsync(r => 
                r.RoomId == roomId &&
                
                // Only check active reservations (Pending or Approved)
                (r.Status == ReservationStatus.Pending || r.Status == ReservationStatus.Approved) &&
                
                // Time overlap check
                r.StartTime < end && r.EndTime > start
            );
    }
}