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

    [HttpGet("{id}")]
    public async Task<IActionResult> GetReservation(int id)
    {
        var reservation = await _context.Reservations.FindAsync(id);
        if (reservation == null)
            return NotFound($"Reservation with ID {id} not found.");

        var reservationResponseDto = new ReservationResponseDto
        {
            Id = reservation.Id,
            RoomId = reservation.RoomId,
            StartTime = reservation.StartTime,
            EndTime = reservation.EndTime,
            ReservedBy = reservation.ReservedBy,
            Purpose = reservation.Purpose,
            Status = reservation.Status.ToString(),
            CreatedAt = reservation.CreatedAt,
            UpdatedAt = reservation.UpdatedAt
        };

        return Ok(reservationResponseDto);
    }

    [HttpPost]
    public async Task<IActionResult> CreateReservation([FromBody] CreateReservationDto request)
    {
        if (request.RoomId <= 0)
            return BadRequest(new { message = "Invalid Room ID." });

        if (request.StartTime <= DateTime.Now)
            return BadRequest(new { message = "Start time must be in the future." });

        if (request.EndTime <= DateTime.Now)
            return BadRequest(new { message = "End time must be in the future." });

        if (request.StartTime >= request.EndTime)
            return BadRequest(new { message = "End time must be after start time." });

        if (!await _reservationService.IsRoomAvailableAsync(request.RoomId, request.StartTime, request.EndTime))
            return Conflict(new { message = "Room is not available for the selected time." });

        if (!Enum.TryParse<ReservationStatus>(request.Status, ignoreCase: true, out var status))
            return BadRequest(new { message = "Invalid reservation status." });

        var reservation = new Reservation
        {
            RoomId = request.RoomId,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            ReservedBy = request.ReservedBy,
            Purpose = request.Purpose,
            Status = status,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync();

        var reservationResponseDto = new ReservationResponseDto
        {
            Id = reservation.Id,
            RoomId = reservation.RoomId,
            StartTime = reservation.StartTime,
            EndTime = reservation.EndTime,
            ReservedBy = reservation.ReservedBy,
            Purpose = reservation.Purpose,
            Status = reservation.Status.ToString(),
            CreatedAt = reservation.CreatedAt,
            UpdatedAt = reservation.UpdatedAt
        };

        return CreatedAtAction(nameof(GetReservation), new { id = reservation.Id }, reservationResponseDto);
    }
}