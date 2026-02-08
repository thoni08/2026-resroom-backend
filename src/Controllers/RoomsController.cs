using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResRoomApi.Data;
using ResRoomApi.DTOs.Rooms;
using ResRoomApi.Helpers;
using ResRoomApi.Models;

namespace ResRoomApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RoomsController : ControllerBase
{
    private readonly ResRoomApiContext _context;

    public RoomsController(ResRoomApiContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetRooms([FromQuery] RoomParams roomParams)
    {
        var query = _context.Rooms.AsQueryable();

        if (!string.IsNullOrEmpty(roomParams.SearchTerm))
        {
            var searchTerm = roomParams.SearchTerm.ToLower();
            query = query.Where(r => r.Name.ToLower().Contains(searchTerm) || 
                                     r.Location.ToLower().Contains(searchTerm));
        }

        if (roomParams.MinCapacity.HasValue)
            query = query.Where(r => r.Capacity >= roomParams.MinCapacity.Value);

        query = roomParams.SortBy?.ToLower() switch
        {
            "capacity" => roomParams.SortDirection == "desc" 
                ? query.OrderByDescending(r => r.Capacity) 
                : query.OrderBy(r => r.Capacity),
            "location" => roomParams.SortDirection == "desc" 
                ? query.OrderByDescending(r => r.Location) 
                : query.OrderBy(r => r.Location),
            _ => query.OrderBy(r => r.Name) // Default sort
        };

        var dtoQuery = query.Select(r => new RoomResponseDto
        {
            Id = r.Id,
            Name = r.Name,
            Capacity = r.Capacity,
            Location = r.Location,
            Description = r.Description,
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt
        });

        var pagedList = await PagedList<RoomResponseDto>.CreateAsync(
            dtoQuery,
            roomParams.PageNumber,
            roomParams.PageSize
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
    public async Task<IActionResult> GetRoom(int id)
    {
        var room = await _context.Rooms.FindAsync(id);
        if (room == null)
            return NotFound($"Room with ID {id} not found.");

        var roomResponseDto = new RoomResponseDto
        {
            Id = room.Id,
            Name = room.Name,
            Capacity = room.Capacity,
            Location = room.Location,
            Description = room.Description,
            CreatedAt = room.CreatedAt,
            UpdatedAt = room.UpdatedAt
        };

        return Ok(roomResponseDto);
    }

    [HttpPost]
    public async Task<IActionResult> CreateRoom([FromBody] CreateRoomDto request)
    {
        var room = new Room
        {
            Name = request.Name,
            Capacity = request.Capacity,
            Location = request.Location,
            Description = request.Description,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();

        var roomResponseDto = new RoomResponseDto
        {
            Id = room.Id,
            Name = room.Name,
            Capacity = room.Capacity,
            Location = room.Location,
            Description = room.Description,
            CreatedAt = room.CreatedAt,
            UpdatedAt = room.UpdatedAt
        };

        return CreatedAtAction(nameof(GetRooms), new { id = room.Id }, roomResponseDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRoom(int id, [FromBody] UpdateRoomDto request)
    {
        var room = await _context.Rooms.FindAsync(id);
        if (room == null)
            return NotFound($"Room with ID {id} not found.");

        if (request.Name != null)
            room.Name = request.Name;
        
        if (request.Capacity.HasValue)
            room.Capacity = request.Capacity.Value;

        if (request.Location != null)
            room.Location = request.Location;

        if (request.Description != null)
            room.Description = request.Description;

        room.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();

        var dto = new RoomResponseDto
        {
            Id = room.Id,
            Name = room.Name,
            Capacity = room.Capacity,
            Location = room.Location,
            Description = room.Description,
            CreatedAt = room.CreatedAt,
            UpdatedAt = room.UpdatedAt
        };

        return Ok(new { message = $"Room with ID {id} updated successfully" });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRoom(int id)
    {
        var room = await _context.Rooms.FindAsync(id);
        if (room == null)
            return NotFound($"Room with ID {id} not found.");

        room.DeletedAt = DateTime.Now;
        await _context.SaveChangesAsync();

        return Ok(new { message = $"Room with ID {id} deleted successfully" });
    }
}