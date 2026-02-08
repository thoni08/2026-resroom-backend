using System.ComponentModel.DataAnnotations;

namespace ResRoomApi.DTOs.Reservations;

public class CreateReservationDto
{
    [Required(ErrorMessage = "A room must be selected.")]
    [Range(1, int.MaxValue, ErrorMessage = "Invalid Room ID.")]
    public int RoomId { get; set; }

    [Required(ErrorMessage = "Start time is required.")]
    [DataType(DataType.DateTime, ErrorMessage = "Start time must be a valid date and time.")]
    public DateTime StartTime { get; set; }

    [Required(ErrorMessage = "End time is required.")]
    [DataType(DataType.DateTime, ErrorMessage = "End time must be a valid date and time.")]
    public DateTime EndTime { get; set; }

    [Required(ErrorMessage = "Reserver name is required.")]
    [MaxLength(100, ErrorMessage = "Reserver name cannot exceed 100 characters.")]
    public string ReservedBy { get; set; } = string.Empty;

    [Required(ErrorMessage = "Reservation purpose is required.")]
    [MaxLength(300, ErrorMessage = "Reservation purpose cannot exceed 300 characters.")]
    public string Purpose { get; set; } = string.Empty;

    [Required(ErrorMessage = "Reservation status is required.")]
    public string Status { get; set; } = "Pending";
}