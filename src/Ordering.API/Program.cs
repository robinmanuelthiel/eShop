// Einstiegspunkt der Anwendung
var builder = WebApplication.CreateBuilder(args);

// Fügt Standarddienste für das Service Mesh hinzu
builder.AddServiceDefaults();
// Registriert anwendungsspezifische Dienste
builder.AddApplicationServices();
// Fügt Unterstützung für ProblemDetails (RFC 7807) hinzu
builder.Services.AddProblemDetails();

// Aktiviert API-Versionierung
var withApiVersioning = builder.Services.AddApiVersioning();

// Fügt OpenAPI/Swagger mit Versionierung hinzu
builder.AddDefaultOpenApi(withApiVersioning);

// Baut die Anwendung
var app = builder.Build();

// Mappt Standardendpunkte (z.B. Health Checks)
app.MapDefaultEndpoints();

// Erstellt eine versionierte API-Gruppe für Bestellungen
var orders = app.NewVersionedApi("Orders");

// Mappt die V1-Endpunkte für Bestellungen und verlangt Authentifizierung
orders.MapOrdersApiV1()
      .RequireAuthorization();

// Aktiviert OpenAPI/Swagger Middleware
app.UseDefaultOpenApi();
// Startet die Anwendung
app.Run();
