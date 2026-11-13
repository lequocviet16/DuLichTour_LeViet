using LeVietTravel.BookingService.Data;
using LeVietTravel.Common.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "LeViet Travel - Booking Service API",
        Version = "v1",
        Description = "Microservice đặt tour du lịch và thanh toán VietQR (.NET 8)"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Nhập Access Token vào đây để xác thực:"
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

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<BookingDbContext>(options =>
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
        options.UseInMemoryDatabase("LeVietTravel_BookingDb");
    }
});

builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        var context = services.GetRequiredService<BookingDbContext>();
        BookingDbInitializer.Initialize(context);
        logger.LogInformation("Database BookingDb đã được khởi tạo và nạp dữ liệu mẫu.");
    }
    catch (Exception ex)
    {
        logger.LogWarning("Không kết nối được SQL Server cục bộ ({Error}). Tự động dùng In-Memory Database...", ex.Message);
        var inMemoryOptions = new DbContextOptionsBuilder<BookingDbContext>()
            .UseInMemoryDatabase("LeVietTravel_BookingDb")
            .Options;
        using var fallbackContext = new BookingDbContext(inMemoryOptions);
        BookingDbInitializer.Initialize(fallbackContext);
    }
}

app.UseCustomMiddlewares();

if (app.Environment.IsDevelopment() || true)
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Booking Service v1"));
}

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
