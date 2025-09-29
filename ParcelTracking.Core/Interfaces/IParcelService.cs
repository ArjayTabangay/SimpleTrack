using ParcelTracking.Core.DTOs;
using ParcelTracking.Core.Models;

namespace ParcelTracking.Core.Interfaces;

public interface IParcelService
{
    Task<ParcelDto?> GetParcelByTrackingNumberAsync(string trackingNumber);
    Task<ParcelDto> CreateParcelAsync(CreateParcelDto createParcelDto);
    Task<ParcelDto?> UpdateParcelStatusAsync(Guid id, UpdateParcelStatusDto updateStatusDto);
    Task<IEnumerable<ParcelDto>> GetAllParcelsAsync();
}