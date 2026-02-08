using System.ComponentModel.DataAnnotations;

namespace ResRoomApi.Models;

public class Room
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public int Capacity { get; set; }

    [Required]
    [MaxLength(300)]
    public string Location { get; set; } = string.Empty;

    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}