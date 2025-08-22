var builder = WebApplication.CreateBuilder(args);

// CORS aktivieren
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularDev", policy =>
    {
        policy.WithOrigins("http://localhost:4200") // Angular Dev-Server
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddScoped<FlightBackend.Services.LufthansaService>();

var app = builder.Build();

// Reihenfolge ist wichtig
app.UseHttpsRedirection();
app.UseCors("AllowAngularDev");
app.UseAuthorization();

app.MapControllers();

app.Run();
