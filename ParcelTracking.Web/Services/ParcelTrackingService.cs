using Microsoft.AspNetCore.SignalR.Client;
using ParcelTracking.Core.DTOs;
using System.Net.Http.Json;

namespace ParcelTracking.Web.Services;

public class ParcelTrackingService : IAsyncDisposable
{
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;
    private readonly ILogger<ParcelTrackingService> _logger;
    private HubConnection? _hubConnection;

    public ParcelTrackingService(HttpClient httpClient, AuthService authService, ILogger<ParcelTrackingService> logger)
    {
        _httpClient = httpClient;
        _authService = authService;
        _logger = logger;
    }

    public event Action<ParcelDto>? ParcelStatusUpdated;
    public event Action<ParcelDto>? ParcelCreated;

    public async Task<ParcelDto?> GetParcelAsync(string trackingNumber)
    {
        try
        {
            if (!_authService.IsAuthenticated)
            {
                throw new InvalidOperationException("User is not authenticated");
            }

            var response = await _httpClient.GetAsync($"api/parcel/{trackingNumber}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ParcelDto>();
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
            else
            {
                _logger.LogWarning("Failed to get parcel {TrackingNumber}. Status: {StatusCode}", 
                    trackingNumber, response.StatusCode);
                return null;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting parcel {TrackingNumber}", trackingNumber);
            return null;
        }
    }

    public async Task<IEnumerable<ParcelDto>?> GetAllParcelsAsync()
    {
        try
        {
            if (!_authService.IsAuthenticated)
            {
                throw new InvalidOperationException("User is not authenticated");
            }

            var response = await _httpClient.GetAsync("api/parcel");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<IEnumerable<ParcelDto>>();
            }
            else
            {
                _logger.LogWarning("Failed to get parcels. Status: {StatusCode}", response.StatusCode);
                return null;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all parcels");
            return null;
        }
    }

    public async Task<ParcelDto?> CreateParcelAsync(CreateParcelDto createParcel)
    {
        try
        {
            if (!_authService.IsAuthenticated)
            {
                throw new InvalidOperationException("User is not authenticated");
            }

            var response = await _httpClient.PostAsJsonAsync("api/parcel", createParcel);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ParcelDto>();
            }
            else
            {
                _logger.LogWarning("Failed to create parcel. Status: {StatusCode}", response.StatusCode);
                return null;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating parcel");
            return null;
        }
    }

    public async Task StartSignalRConnectionAsync(string apiBaseUrl)
    {
        if (_hubConnection != null)
        {
            await _hubConnection.DisposeAsync();
        }

        _hubConnection = new HubConnectionBuilder()
            .WithUrl($"{apiBaseUrl}/hub/parcels", options =>
            {
                if (_authService.IsAuthenticated)
                {
                    options.AccessTokenProvider = () => Task.FromResult(_authService.Token);
                }
            })
            .WithAutomaticReconnect()
            .Build();

        _hubConnection.On<ParcelDto>("ParcelStatusUpdated", (parcel) =>
        {
            _logger.LogInformation("Received parcel status update for {TrackingNumber}", parcel.TrackingNumber);
            ParcelStatusUpdated?.Invoke(parcel);
        });

        _hubConnection.On<ParcelDto>("ParcelCreated", (parcel) =>
        {
            _logger.LogInformation("Received new parcel notification for {TrackingNumber}", parcel.TrackingNumber);
            ParcelCreated?.Invoke(parcel);
        });

        try
        {
            await _hubConnection.StartAsync();
            _logger.LogInformation("SignalR connection established");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting SignalR connection");
        }
    }

    public async Task JoinTrackingGroupAsync(string trackingNumber)
    {
        if (_hubConnection?.State == HubConnectionState.Connected)
        {
            try
            {
                await _hubConnection.InvokeAsync("JoinTrackingGroup", trackingNumber);
                _logger.LogInformation("Joined tracking group for {TrackingNumber}", trackingNumber);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error joining tracking group for {TrackingNumber}", trackingNumber);
            }
        }
    }

    public async Task LeaveTrackingGroupAsync(string trackingNumber)
    {
        if (_hubConnection?.State == HubConnectionState.Connected)
        {
            try
            {
                await _hubConnection.InvokeAsync("LeaveTrackingGroup", trackingNumber);
                _logger.LogInformation("Left tracking group for {TrackingNumber}", trackingNumber);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error leaving tracking group for {TrackingNumber}", trackingNumber);
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_hubConnection != null)
        {
            await _hubConnection.DisposeAsync();
        }
    }
}