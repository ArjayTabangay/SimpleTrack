using System.Net.Http.Headers;

namespace ParcelTracking.Web.Services;

public class JwtAuthorizationHandler : DelegatingHandler
{
    private readonly AuthService _authService;
    private readonly ILogger<JwtAuthorizationHandler> _logger;

    public JwtAuthorizationHandler(AuthService authService, ILogger<JwtAuthorizationHandler> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        try
        {
            if (_authService.IsAuthenticated && !request.Headers.Contains("Authorization"))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _authService.Token);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to append JWT authorization header");
        }

        return base.SendAsync(request, cancellationToken);
    }
}