using Microsoft.EntityFrameworkCore;
using ShortenURL.DataBase;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();

// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Handle Railway's DATABASE_URL environment variable
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

if (!string.IsNullOrEmpty(databaseUrl))
{
    // Parse DATABASE_URL format: postgres://user:password@host:port/dbname
    var uri = new Uri(databaseUrl);
    var npgsqlconnectionString = $"Host={uri.Host};Port={uri.Port};Database={uri.LocalPath.TrimStart('/')};Username={uri.UserInfo.Split(':')[0]};Password={uri.UserInfo.Split(':')[1]};SSL Mode=Require;Trust Server Certificate=true";
    
    connectionString = npgsqlconnectionString;
}

Console.WriteLine($"Connection String (raw) = '{connectionString}'");


builder.Services.AddDbContext<UrlDbContext>(options => options.UseNpgsql(connectionString));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configure pipeline
app.UseRouting();
app.MapControllers();

app.Run();