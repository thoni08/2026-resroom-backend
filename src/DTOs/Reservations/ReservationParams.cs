namespace ResRoomApi.DTOs.Reservations;

public class ReservationParams : BaseParams
{
    // Filters
    public DateOnly? MinDate { get; set; }
    public DateOnly? MaxDate { get; set; }
    public string? ReservedBy { get; set; }
    public int? RoomId { get; set; }
    public string? Status { get; set; }

    // Display Options
    public string? View { get; set; }
}