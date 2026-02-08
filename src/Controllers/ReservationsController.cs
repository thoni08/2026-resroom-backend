using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResRoomApi.Data;
using ResRoomApi.DTOs.Reservations;
using ResRoomApi.Models;
using ResRoomApi.Services;

namespace ResRoomApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReservationsController : ControllerBase
{
    private readonly ResRoomApiContext _context;
    private readonly IReservationService _reservationService;

    public ReservationsController(ResRoomApiContext context, IReservationService reservationService)
    {
        _context = context;
        _reservationService = reservationService;
    }

    [HttpGet]
    public async Task<IActionResult> GetReservations([FromQuery] string? view = "active")
    {
        var query = _context.Reservations.AsQueryable();

        var currentTime = DateTime.Now;

        if (view == "history")
        {
            // History: Past dates OR Rejected/Cancelled status
            query = query.Where(r => r.EndTime < currentTime || 
                                     r.Status == ReservationStatus.Rejected || 
                                     r.Status == ReservationStatus.Cancelled);
                                    
            // Sort history by newest first
            query = query.OrderByDescending(r => r.StartTime);
        }
        else
        {
            // List: Future dates AND Active status
            query = query.Where(r => r.EndTime >= currentTime && 
                                     (r.Status == ReservationStatus.Pending || 
                                     r.Status == ReservationStatus.Approved));
                                    
            // Sort upcoming list by soonest first
            query = query.OrderBy(r => r.StartTime);
        }

        var reservations = await query.ToListAsync();
        
        var reservationResponseDtos = reservations.Select(r => new ReservationResponseDto
        {
            Id = r.Id,
            RoomId = r.RoomId,
            StartTime = r.StartTime,
            EndTime = r.EndTime,
            ReservedBy = r.ReservedBy,
            Purpose = r.Purpose,
            Status = r.Status.ToString(),
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt
        }).ToList();

        return Ok(reservationResponseDtos);
    }
}