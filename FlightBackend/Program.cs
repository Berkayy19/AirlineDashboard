var builder = WebApplication.CreateBuilder(args);

// CORS fuer Angular Dev erlauben
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddScoped<FlightBackend.Services.LufthansaService>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors(); // wichtig: vor Authorization
app.UseAuthorization();

app.MapControllers();

app.Run();
