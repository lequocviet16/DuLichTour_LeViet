using System.Text;
using BLL;
using DAL;
using LeViet_Travel_API.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// 1. Cấu hình Cơ sở dữ liệu (SQL Server với Fallback In-Memory)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Server=localhost;Database=LeVietTravel_DevDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true";

builder.Services.AddDbContext<LeVietTravelDbContext>(options =>
{
    try
    {
        options.UseSqlServer(connectionString);
    }
    catch
    {
        options.UseInMemoryDatabase("LeVietTravel_DevDb");
    }
});

// 2. Đăng ký các dịch vụ BLL và DAL
builder.Services.AddBLLServices();

// 3. Cấu hình JWT Authentication
var secretKey = builder.Configuration["Jwt:SecretKey"] ?? "LeVietTravel_Super_Secret_Key_For_Jwt_Authentication_2026_Minimum_32_Chars";
var issuer = builder.Configuration["Jwt:Issuer"] ?? "LeVietTravel";
var audience = builder.Configuration["Jwt:Audience"] ?? "LeVietTravelApp";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ValidateIssuer = true,
        ValidIssuer = issuer,
        ValidateAudience = true,
        ValidAudience = audience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// 4. Cấu hình CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// 5. Cấu hình Swagger với JWT Bearer
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "LeViet Travel API",
        Version = "v1",
        Description = "Hệ thống REST API Quản lý & Đặt Tour Du Lịch (Kiến trúc N-Tier ASP.NET Core 8.0)"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Nhập token JWT theo định dạng: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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

var app = builder.Build();

// 6. Tự động khởi tạo Database và Seed dữ liệu mẫu
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<LeVietTravelDbContext>();
    try
    {
        if (!context.Database.CanConnect())
        {
            // Nếu SQL Server chưa bật, chuyển sang InMemory
            Console.WriteLine("[INFO] SQL Server chưa kết nối được, hệ thống tự động chạy trên In-Memory Database...");
            var inMemOptions = new DbContextOptionsBuilder<LeVietTravelDbContext>()
                .UseInMemoryDatabase("LeVietTravel_DevDb")
                .Options;
            var inMemContext = new LeVietTravelDbContext(inMemOptions);
            DbInitializer.Initialize(inMemContext);
        }
        else
        {
            DbInitializer.Initialize(context);
        }
    }
    catch
    {
        try
        {
            DbInitializer.Initialize(context);
        }
        catch { }
    }
}

// 7. Middlewares
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseCors("AllowAll");

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "LeViet Travel API v1");
    c.RoutePrefix = "swagger";
});

// 8. Cấu hình phục vụ giao diện Web Frontend trực tiếp
var frontendPath = Path.Combine(app.Environment.ContentRootPath, "..", "Frontend");
if (!Directory.Exists(frontendPath))
{
    frontendPath = Path.Combine(app.Environment.ContentRootPath, "Frontend");
}

if (Directory.Exists(frontendPath))
{
    var fileProvider = new PhysicalFileProvider(frontendPath);
    app.UseDefaultFiles(new DefaultFilesOptions
    {
        FileProvider = fileProvider,
        RequestPath = ""
    });
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = fileProvider,
        RequestPath = ""
    });
}
else
{
    app.UseDefaultFiles();
    app.UseStaticFiles();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

Console.WriteLine("====================================================================");
Console.WriteLine("  LEVIET TRAVEL API ĐANG CHẠY TẠI: http://localhost:5001");
Console.WriteLine("  TÀI LIỆU SWAGGER:               http://localhost:5001/swagger");
Console.WriteLine("  WEBSITE KHÁCH HÀNG:             http://localhost:5001");
Console.WriteLine("====================================================================");

app.Run();
