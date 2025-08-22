using FlightBackend.Services; // wichtig
var builder = WebApplication.CreateBuilder(args);

// CORS fuer Angular Dev
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(p =>
        p.WithOrigins("http://localhost:4200")
         .AllowAnyHeader()
         .AllowAnyMethod());
});

builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();

// eigene Services registrieren
builder.Services.AddScoped<LufthansaAuthService>();
builder.Services.AddScoped<LufthansaService>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();
app.MapControllers();

app.Run();
