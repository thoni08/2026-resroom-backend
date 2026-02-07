using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResRoomApi.Data;
using ResRoomApi.DTOs.Rooms;
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
    public async Task<IActionResult> GetRooms()
    {
        var rooms = await _context.Rooms.ToListAsync();
        
        var roomResponseDtos = rooms.Select(r => new RoomResponseDto
        {
            Id = r.Id,
            Name = r.Name,
            Capacity = r.Capacity,
            Location = r.Location,
            Description = r.Description,
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt
        }).ToList();

        return Ok(roomResponseDtos);
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
}