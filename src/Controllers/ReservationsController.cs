using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResRoomApi.Data;
using ResRoomApi.DTOs.Reservations;
using ResRoomApi.Helpers;
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
    public async Task<IActionResult> GetReservations([FromQuery] ReservationParams reservationParams)
    {
        var query = _context.Reservations.AsQueryable();

        if (!string.IsNullOrEmpty(reservationParams.SearchTerm))
        {
            var searchTerm = reservationParams.SearchTerm.ToLower();
            query = query.Where(r => r.Purpose.ToLower().Contains(searchTerm));
        }

        if (reservationParams.MinDate.HasValue)
        {
            var minDateTime = reservationParams.MinDate.Value.ToDateTime(new TimeOnly(0, 0));
            query = query.Where(r => r.StartTime >= minDateTime);
        }

        if (reservationParams.MaxDate.HasValue)
        {
            var maxDateTime = reservationParams.MaxDate.Value.ToDateTime(new TimeOnly(23, 59));
            query = query.Where(r => r.EndTime <= maxDateTime);
        }

        if (!string.IsNullOrEmpty(reservationParams.ReservedBy))
        {
            var reservedByLower = reservationParams.ReservedBy.ToLower();
            query = query.Where(r => r.ReservedBy.ToLower().Contains(reservedByLower));
        }

        if (reservationParams.RoomId.HasValue)
            query = query.Where(r => r.RoomId == reservationParams.RoomId.Value);

        if (!string.IsNullOrEmpty(reservationParams.Status))
        {
            if (Enum.TryParse<ReservationStatus>(reservationParams.Status, ignoreCase: true, out var status))
            {
                query = query.Where(r => r.Status == status);
            }
        }

        var currentTime = DateTime.Now;
        query = reservationParams.View?.ToLower() switch
        {
            "history" => query.Where(r => r.EndTime < currentTime || 
                                          r.Status == ReservationStatus.Rejected || 
                                          r.Status == ReservationStatus.Cancelled)
                             .OrderByDescending(r => r.StartTime),
            "active" => query.Where(r => r.EndTime >= currentTime && 
                                         (r.Status == ReservationStatus.Pending || 
                                          r.Status == ReservationStatus.Approved))
                            .OrderBy(r => r.StartTime),
            _ => query.OrderByDescending(r => r.StartTime)
        };

        query = reservationParams.SortBy?.ToLower() switch
        {
            "roomname" => reservationParams.SortDirection == "desc" 
                ? query.OrderByDescending(r => r.Room != null ? r.Room.Name : string.Empty) 
                : query.OrderBy(r => r.Room != null ? r.Room.Name : string.Empty),
            "starttime" => reservationParams.SortDirection == "desc" 
                ? query.OrderByDescending(r => r.StartTime) 
                : query.OrderBy(r => r.StartTime),
            "endtime" => reservationParams.SortDirection == "desc" 
                ? query.OrderByDescending(r => r.EndTime) 
                : query.OrderBy(r => r.EndTime),
            "reservedby" => reservationParams.SortDirection == "desc" 
                ? query.OrderByDescending(r => r.ReservedBy) 
                : query.OrderBy(r => r.ReservedBy),
            "status" => reservationParams.SortDirection == "desc" 
                ? query.OrderByDescending(r => r.Status) 
                : query.OrderBy(r => r.Status),
            _ => query // Default sort already applied above
        };
        
        var dtoQuery = query.Select(r => new ReservationResponseDto
        {
            Id = r.Id,
            RoomId = r.RoomId,
            RoomName = r.Room != null ? r.Room.Name : string.Empty,
            StartTime = r.StartTime,
            EndTime = r.EndTime,
            ReservedBy = r.ReservedBy,
            Purpose = r.Purpose,
            Status = r.Status.ToString(),
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt
        });

        var pagedList = await PagedList<ReservationResponseDto>.CreateAsync(
            dtoQuery,
            reservationParams.PageNumber,
            reservationParams.PageSize
        );

        var paginationMetadata = new
        {
            pagedList.TotalCount,
            pagedList.PageSize,
            pagedList.CurrentPage,
            pagedList.TotalPages,
            pagedList.HasNext,
            pagedList.HasPrevious
        };

        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

        return Ok(pagedList);
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

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateReservation(int id, [FromBody] UpdateReservationDto request)
    {
        var reservation = await _context.Reservations.FindAsync(id);
        if (reservation == null)
            return NotFound($"Reservation with ID {id} not found.");

        var newRoomId = request.RoomId ?? reservation.RoomId;
        var newStart = request.StartTime ?? reservation.StartTime;
        var newEnd = request.EndTime ?? reservation.EndTime;

        var timeOrRoomChanged =
            (request.StartTime.HasValue && request.StartTime.Value != reservation.StartTime) ||
            (request.EndTime.HasValue && request.EndTime.Value != reservation.EndTime) ||
            (request.RoomId.HasValue && request.RoomId.Value != reservation.RoomId);

        if (request.RoomId.HasValue && request.RoomId <= 0)
            return BadRequest(new { message = "Invalid Room ID." });
        else
            reservation.RoomId = newRoomId;

        if (request.StartTime.HasValue)
        {
            if (newStart <= DateTime.Now)
                return BadRequest(new { message = "Start time must be in the future." });

            reservation.StartTime = newStart;
        }

        if (request.EndTime.HasValue)
        {
            if (newEnd <= DateTime.Now)
                return BadRequest(new { message = "End time must be in the future." });

            reservation.EndTime = newEnd;
        }

        if (newStart >= newEnd)
            return BadRequest(new { message = "End time must be after start time." });

        if (timeOrRoomChanged &&
            !await _reservationService.IsRoomAvailableAsync(newRoomId, newStart, newEnd))
            return Conflict(new { message = "Room is not available for the selected time." });

        if (!string.IsNullOrEmpty(request.ReservedBy))
            reservation.ReservedBy = request.ReservedBy;

        if (!string.IsNullOrEmpty(request.Purpose))
            reservation.Purpose = request.Purpose;

        if (!string.IsNullOrEmpty(request.Status))
        {
            if (!Enum.TryParse<ReservationStatus>(request.Status, ignoreCase: true, out var status))
                return BadRequest(new { message = "Invalid reservation status." });

            reservation.Status = status;
        }

        reservation.UpdatedAt = DateTime.Now;
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

        return Ok(new { reservationResponseDto, test = timeOrRoomChanged });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReservation(int id)
    {
        var reservation = await _context.Reservations.FindAsync(id);
        if (reservation == null)
            return NotFound($"Reservation with ID {id} not found.");

        reservation.DeletedAt = DateTime.Now;
        await _context.SaveChangesAsync();

        return Ok(new { message = $"Reservation with ID {id} deleted successfully." });
    }
}