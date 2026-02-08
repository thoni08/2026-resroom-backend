namespace ResRoomApi.DTOs.Rooms;
public class RoomResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int Capacity { get; set; }
    public string Location { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}