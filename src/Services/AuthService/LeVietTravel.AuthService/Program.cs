using LeVietTravel.AuthService.Data;
using LeVietTravel.AuthService.Services;
using LeVietTravel.Common.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger with JWT Support
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "LeViet Travel - Auth Service API",
        Version = "v1",
        Description = "Microservice xử lý xác thực, cấp JWT & Refresh Token, phân quyền người dùng (.NET 8)"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Nhập Access Token vào ô bên dưới (không cần gõ từ khóa Bearer):"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Database setup: SQL Server with fallback to InMemory for smooth execution
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AuthDbContext>(options =>
{
    try
    {
        options.UseSqlServer(connectionString, sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(maxRetryCount: 2, maxRetryDelay: TimeSpan.FromSeconds(2), errorNumbersToAdd: null);
        });
    }
    catch
    {
        options.UseInMemoryDatabase("LeVietTravel_AuthDb");
    }
});

// Services
builder.Services.AddScoped<ITokenService, TokenService>();

// Common JWT Authentication & Authorization
builder.Services.AddJwtAuthentication(builder.Configuration);

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Auto-seed database (with fallback to InMemory if SQL Server host is unavailable)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        var context = services.GetRequiredService<AuthDbContext>();
        DbInitializer.Initialize(context);
        logger.LogInformation("Database AuthDb đã được khởi tạo và nạp dữ liệu mẫu thành công.");
    }
    catch (Exception ex)
    {
        logger.LogWarning("Không kết nối được SQL Server cục bộ ({Error}). Tự động chuyển sang In-Memory Database để chạy ngay...", ex.Message);
        var inMemoryOptions = new DbContextOptionsBuilder<AuthDbContext>()
            .UseInMemoryDatabase("LeVietTravel_AuthDb")
            .Options;
        using var fallbackContext = new AuthDbContext(inMemoryOptions);
        DbInitializer.Initialize(fallbackContext);
    }
}

// Custom Middlewares: Exception Handling & Request Logging
app.UseCustomMiddlewares();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || true)
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Auth Service v1"));
}

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
