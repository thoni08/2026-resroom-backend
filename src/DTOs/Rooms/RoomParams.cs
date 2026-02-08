namespace ResRoomApi.DTOs.Rooms;

public class RoomParams : BaseParams
{
    // Filters
    public int? MinCapacity { get; set; }
    public int? MaxCapacity { get; set; }
}