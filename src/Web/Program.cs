using Web.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Authorization
builder.Services.AddAuthConfiguration();

// DbContext, Repositories
builder.Services.AddDatabaseConfiguration(builder.Configuration);

// Services, Logging, RazorPages
builder.Services.AddServiceConfiguration(builder.Configuration);

var app = builder.Build();

app.AddMiddlewareConfiguration();

// Endpoints
app.MapCustomRoutes();

app.Run();
