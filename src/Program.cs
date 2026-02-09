using Microsoft.EntityFrameworkCore;
using DotNetEnv;
using ResRoomApi.Data;
using ResRoomApi.Services;
using ResRoomApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables from .env file
Env.Load();

// Configure SQL Server connection using environment variable for Entity Framework
var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRINGS");
builder.Services.AddDbContext<ResRoomApiContext>(options =>
    options.UseSqlServer(connectionString 
        ?? throw new InvalidOperationException("Connection string for database connection failed to load from env."),
        sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(10),
                errorNumbersToAdd: null);
        }));

// Configure CORS to allow requests from specific origins
var allowedOrigins = Environment.GetEnvironmentVariable("ALLOWED_ORIGINS")?.Split(';') 
    ?? throw new InvalidOperationException("Allowed origins failed to load from env.");
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy
            .WithOrigins(allowedOrigins.ToArray())
            .AllowAnyHeader()
            .AllowAnyMethod()
            .WithExposedHeaders("X-Pagination");
    });
});

// Enable controllers and JSON enum serialization
builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()));

// Register ReservationService for dependency injection
builder.Services.AddScoped<IReservationService, ReservationService>();

// Enable Swagger for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowSpecificOrigins");

app.MapControllers();

app.Run();
