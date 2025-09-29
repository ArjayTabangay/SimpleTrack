using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using ParcelTracking.Api.Hubs;
using ParcelTracking.Core.DTOs;
using ParcelTracking.Core.Interfaces;

namespace ParcelTracking.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ParcelController : ControllerBase
{
    private readonly IParcelService _parcelService;
    private readonly IHubContext<ParcelHub> _hubContext;
    private readonly ILogger<ParcelController> _logger;

    public ParcelController(
        IParcelService parcelService, 
        IHubContext<ParcelHub> hubContext,
        ILogger<ParcelController> logger)
    {
        _parcelService = parcelService;
        _hubContext = hubContext;
        _logger = logger;
    }

    [HttpGet("{trackingNumber}")]
    public async Task<ActionResult<ParcelDto>> GetParcel(string trackingNumber)
    {
        if (string.IsNullOrEmpty(trackingNumber))
        {
            return BadRequest("Tracking number is required");
        }

        var parcel = await _parcelService.GetParcelByTrackingNumberAsync(trackingNumber);
        if (parcel == null)
        {
            return NotFound($"Parcel with tracking number {trackingNumber} not found");
        }

        return Ok(parcel);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ParcelDto>>> GetAllParcels()
    {
        var parcels = await _parcelService.GetAllParcelsAsync();
        return Ok(parcels);
    }

    [HttpPost]
    public async Task<ActionResult<ParcelDto>> CreateParcel([FromBody] CreateParcelDto createParcelDto)
    {
        if (string.IsNullOrEmpty(createParcelDto.TrackingNumber))
        {
            return BadRequest("Tracking number is required");
        }

        try
        {
            var parcel = await _parcelService.CreateParcelAsync(createParcelDto);
            
            // Broadcast new parcel creation via SignalR
            await _hubContext.Clients.All.SendAsync("ParcelCreated", parcel);
            await _hubContext.Clients.Group($"tracking_{parcel.TrackingNumber}")
                .SendAsync("ParcelStatusUpdated", parcel);

            _logger.LogInformation("Created new parcel with tracking number {TrackingNumber}", parcel.TrackingNumber);
            
            return CreatedAtAction(nameof(GetParcel), new { trackingNumber = parcel.TrackingNumber }, parcel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating parcel with tracking number {TrackingNumber}", createParcelDto.TrackingNumber);
            return StatusCode(500, "An error occurred while creating the parcel");
        }
    }

    [HttpPut("{id}/status")]
    public async Task<ActionResult<ParcelDto>> UpdateParcelStatus(Guid id, [FromBody] UpdateParcelStatusDto updateStatusDto)
    {
        if (string.IsNullOrEmpty(updateStatusDto.Status))
        {
            return BadRequest("Status is required");
        }

        try
        {
            var parcel = await _parcelService.UpdateParcelStatusAsync(id, updateStatusDto);
            if (parcel == null)
            {
                return NotFound($"Parcel with ID {id} not found");
            }

            // Broadcast status update via SignalR
            await _hubContext.Clients.Group($"tracking_{parcel.TrackingNumber}")
                .SendAsync("ParcelStatusUpdated", parcel);
            await _hubContext.Clients.All.SendAsync("ParcelUpdated", parcel);

            _logger.LogInformation("Updated parcel {Id} status to {Status}", id, updateStatusDto.Status);
            
            return Ok(parcel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating parcel {Id} status", id);
            return StatusCode(500, "An error occurred while updating the parcel status");
        }
    }
}