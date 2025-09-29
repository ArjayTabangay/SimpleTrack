using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ParcelTracking.Api.Hubs;

[Authorize]
public class ParcelHub : Hub
{
    private readonly ILogger<ParcelHub> _logger;

    public ParcelHub(ILogger<ParcelHub> logger)
    {
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation("Client {ConnectionId} connected to ParcelHub", Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("Client {ConnectionId} disconnected from ParcelHub", Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }

    public async Task JoinTrackingGroup(string trackingNumber)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"tracking_{trackingNumber}");
        _logger.LogInformation("Client {ConnectionId} joined tracking group for {TrackingNumber}", 
            Context.ConnectionId, trackingNumber);
    }

    public async Task LeaveTrackingGroup(string trackingNumber)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"tracking_{trackingNumber}");
        _logger.LogInformation("Client {ConnectionId} left tracking group for {TrackingNumber}", 
            Context.ConnectionId, trackingNumber);
    }
}