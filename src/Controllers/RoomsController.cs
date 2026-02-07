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
}