using System.ComponentModel.DataAnnotations;

namespace ParcelTracking.Core.Models;

public class Parcel
{
    public Guid Id { get; set; }
    
    [Required]
    [StringLength(50)]
    public string TrackingNumber { get; set; } = string.Empty;
    
    [Required]
    [StringLength(50)]
    public string Status { get; set; } = "Pending";
    
    public DateTime UpdatedAt { get; set; }
    
    public DateTime CreatedAt { get; set; }
}