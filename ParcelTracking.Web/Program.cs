using ParcelTracking.Web.Components;
using ParcelTracking.Web.Services;
using MudBlazor.Services;
using MudBlazor;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add MudBlazor services
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
});

// Custom minimal black->gray theme (no green)
var darkGrayTheme = new MudTheme
{
    PaletteDark = new PaletteDark
    {
        Primary = "#212121",
        Secondary = "#424242",
        Background = "#121212",
        Surface = "#1E1E1E",
        AppbarBackground = "#1C1C1C",
        DrawerBackground = "#1A1A1A",
        TextPrimary = "#FFFFFF",
        TextSecondary = "#B0B0B0",
        Divider = "#2E2E2E",
        Success = "#424242", // neutralized
        Info = "#616161",
        Warning = "#757575",
        Error = "#9E9E9E"
    },
    PaletteLight = new PaletteLight
    {
        Primary = "#212121",
        Secondary = "#424242",
        Background = "#F5F5F5",
        Surface = "#FFFFFF",
        AppbarBackground = "#212121",
        DrawerBackground = "#FAFAFA",
        TextPrimary = "#111111",
        TextSecondary = "#4F4F4F",
        Divider = "#E0E0E0",
        Success = "#616161",
        Info = "#757575",
        Warning = "#9E9E9E",
        Error = "#BDBDBD"
    }
};

builder.Services.AddSingleton(darkGrayTheme);

// JWT delegating handler
builder.Services.AddTransient<JwtAuthorizationHandler>();

// Configure HttpClient for API calls
var apiBaseUrl = builder.Configuration.GetValue<string>("ApiSettings:BaseUrl") ?? "https://localhost:5000";
builder.Services.AddHttpClient<AuthService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddHttpClient<ParcelTrackingService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
})
.AddHttpMessageHandler<JwtAuthorizationHandler>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
