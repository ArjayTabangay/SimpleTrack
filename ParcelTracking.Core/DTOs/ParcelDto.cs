namespace ParcelTracking.Core.DTOs;

public class ParcelDto
{
    public Guid Id { get; set; }
    public string TrackingNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime UpdatedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateParcelDto
{
    public string TrackingNumber { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
}

public class UpdateParcelStatusDto
{
    public string Status { get; set; } = string.Empty;
}