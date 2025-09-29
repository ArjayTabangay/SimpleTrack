using ParcelTracking.Core.DTOs;
using System.Net.Http.Json;
using System.Text.Json;

namespace ParcelTracking.Web.Services;

public class AuthService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AuthService> _logger;
    private string? _token;
    private string? _username;

    public AuthService(HttpClient httpClient, ILogger<AuthService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public bool IsAuthenticated => !string.IsNullOrEmpty(_token);
    public string? Username => _username;
    public string? Token => _token;

    public async Task<bool> LoginAsync(string username, string password)
    {
        try
        {
            var loginRequest = new AuthRequestDto
            {
                Username = username,
                Password = password
            };

            var response = await _httpClient.PostAsJsonAsync("api/auth/login", loginRequest);

            if (response.IsSuccessStatusCode)
            {
                var authResponse = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
                if (authResponse != null)
                {
                    _token = authResponse.Token;
                    _username = authResponse.Username;
                    
                    // Set authorization header for future requests
                    _httpClient.DefaultRequestHeaders.Authorization = 
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

                    _logger.LogInformation("User {Username} logged in successfully", username);
                    return true;
                }
            }
            else
            {
                _logger.LogWarning("Login failed for user {Username}. Status: {StatusCode}", 
                    username, response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Login error for user {Username}", username);
        }

        return false;
    }

    public void Logout()
    {
        _token = null;
        _username = null;
        _httpClient.DefaultRequestHeaders.Authorization = null;
        _logger.LogInformation("User logged out");
    }
}