using System.ComponentModel.DataAnnotations;

namespace ParcelTracking.Core.Models;

public class Parcel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    [StringLength(50)]
    public string TrackingNumber { get; set; } = string.Empty;
    
    [Required]
    [StringLength(50)]
    public string Status { get; set; } = "Pending";
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}