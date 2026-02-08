using System.ComponentModel.DataAnnotations;

namespace ResRoomApi.DTOs.Rooms;

public class CreateRoomDto
{
    [Required(ErrorMessage = "Room name is required.")]
    [StringLength(100, ErrorMessage = "Room name cannot exceed 100 characters.")]
    public string Name { get; set; } = null!;

    [Range(1, 1000, ErrorMessage = "Room capacity must be between 1 and 1000.")]
    public int Capacity { get; set; }

    [Required(ErrorMessage = "Room location is required.")]
    [StringLength(300, ErrorMessage = "Room location cannot exceed 300 characters.")]
    public string Location { get; set; } = null!;
    
    public string? Description { get; set; }
}