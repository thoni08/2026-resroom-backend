using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResRoomApi.Models;

public enum ReservationStatus
{
    Pending,
    Approved,
    Rejected,
    Cancelled
}

public class Reservation
{
    [Key]
    public int Id { get; set; }

    [Required]
    [ForeignKey("Room")]
    public int RoomId { get; set; }
    public Room? Room { get; set; }

    [Required]
    public DateTime StartTime { get; set; }
    
    [Required]
    public DateTime EndTime { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string ReservedBy { get; set; } = string.Empty;

    [Required]
    [MaxLength(300)]
    public string Purpose { get; set; } = string.Empty;

    [Required]
    public ReservationStatus Status { get; set; } = ReservationStatus.Pending;
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}