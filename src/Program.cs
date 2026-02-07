using Microsoft.EntityFrameworkCore;
using DotNetEnv;
using ResRoomApi.Data;

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
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Enable Swagger for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowSpecificOrigins");

app.Run();
