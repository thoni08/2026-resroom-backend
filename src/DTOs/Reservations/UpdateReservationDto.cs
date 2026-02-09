using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using ResRoomApi.Helpers.Json;
using ResRoomApi.Models;

namespace ResRoomApi.DTOs.Reservations;

public class UpdateReservationDto
{
    [Range(1, int.MaxValue, ErrorMessage = "Invalid Room ID.")]
    public int? RoomId { get; set; }

    [DataType(DataType.DateTime, ErrorMessage = "Start time must be a valid date and time.")]
    public DateTime? StartTime { get; set; }

    [DataType(DataType.DateTime, ErrorMessage = "End time must be a valid date and time.")]
    public DateTime? EndTime { get; set; }

    [MaxLength(100, ErrorMessage = "Reserver name cannot exceed 100 characters.")]
    public string? ReservedBy { get; set; }

    [MaxLength(300, ErrorMessage = "Reservation purpose cannot exceed 300 characters.")]
    public string? Purpose { get; set; }

    [JsonConverter(typeof(ReservationStatusConverter))]
    public ReservationStatus? Status { get; set; }
}