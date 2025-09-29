using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using ParcelTracking.Api.Data;
using ParcelTracking.Core.DTOs;
using ParcelTracking.Core.Interfaces;
using ParcelTracking.Core.Models;
using System.Text.Json;

namespace ParcelTracking.Api.Services;

public class ParcelService : IParcelService
{
    private readonly AppDbContext _context;
    private readonly IDistributedCache _cache;
    private readonly ILogger<ParcelService> _logger;

    public ParcelService(AppDbContext context, IDistributedCache cache, ILogger<ParcelService> logger)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
    }

    public async Task<ParcelDto?> GetParcelByTrackingNumberAsync(string trackingNumber)
    {
        var cacheKey = $"parcel:{trackingNumber}";
        
        // Try to get from cache first
        var cachedParcel = await _cache.GetStringAsync(cacheKey);
        if (!string.IsNullOrEmpty(cachedParcel))
        {
            _logger.LogInformation("Retrieved parcel {TrackingNumber} from cache", trackingNumber);
            return JsonSerializer.Deserialize<ParcelDto>(cachedParcel);
        }

        // Get from database
        var parcel = await _context.Parcels
            .FirstOrDefaultAsync(p => p.TrackingNumber == trackingNumber);

        if (parcel == null)
            return null;

        var parcelDto = MapToDto(parcel);

        // Cache the result
        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
        };
        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(parcelDto), cacheOptions);

        _logger.LogInformation("Retrieved parcel {TrackingNumber} from database and cached", trackingNumber);
        return parcelDto;
    }

    public async Task<ParcelDto> CreateParcelAsync(CreateParcelDto createParcelDto)
    {
        var parcel = new Parcel
        {
            Id = Guid.NewGuid(),
            TrackingNumber = createParcelDto.TrackingNumber,
            Status = createParcelDto.Status,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Parcels.Add(parcel);
        await _context.SaveChangesAsync();

        // Invalidate cache for this tracking number
        var cacheKey = $"parcel:{parcel.TrackingNumber}";
        await _cache.RemoveAsync(cacheKey);

        _logger.LogInformation("Created new parcel with tracking number {TrackingNumber}", parcel.TrackingNumber);
        return MapToDto(parcel);
    }

    public async Task<ParcelDto?> UpdateParcelStatusAsync(Guid id, UpdateParcelStatusDto updateStatusDto)
    {
        var parcel = await _context.Parcels.FindAsync(id);
        if (parcel == null)
            return null;

        parcel.Status = updateStatusDto.Status;
        parcel.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        // Invalidate cache for this tracking number
        var cacheKey = $"parcel:{parcel.TrackingNumber}";
        await _cache.RemoveAsync(cacheKey);

        _logger.LogInformation("Updated parcel {Id} status to {Status}", id, updateStatusDto.Status);
        return MapToDto(parcel);
    }

    public async Task<IEnumerable<ParcelDto>> GetAllParcelsAsync()
    {
        var parcels = await _context.Parcels.ToListAsync();
        return parcels.Select(MapToDto);
    }

    private static ParcelDto MapToDto(Parcel parcel)
    {
        return new ParcelDto
        {
            Id = parcel.Id,
            TrackingNumber = parcel.TrackingNumber,
            Status = parcel.Status,
            CreatedAt = parcel.CreatedAt,
            UpdatedAt = parcel.UpdatedAt
        };
    }
}