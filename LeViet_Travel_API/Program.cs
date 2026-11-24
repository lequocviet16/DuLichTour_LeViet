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

// 1. Cấu hình Cơ sở dữ liệu (Tự động nhận diện MySQL, SQL Server, hoặc Fallback In-Memory)
var rawConnectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Server=localhost;Port=3306;Database=leviet_travel_net;User=root;Password=123456;";

bool isMysqlOnline = false;
bool isSqlServerOnline = false;

// A. Kiểm tra kết nối MySQL
try
{
    using var mySqlConn = new MySqlConnector.MySqlConnection(rawConnectionString);
    mySqlConn.Open();
    isMysqlOnline = true;
}
catch
{
    isMysqlOnline = false;
}

if (isMysqlOnline)
{
    Console.WriteLine("[DATABASE] Đã kết nối thành công tới MySQL Database (MySQL 8.0).");
    builder.Services.AddDbContext<LeVietTravelDbContext>(options =>
        options.UseMySql(rawConnectionString, ServerVersion.AutoDetect(rawConnectionString)));
}
else
{
    // B. Kiểm tra kết nối SQL Server
    try
    {
        var testBuilder = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(rawConnectionString)
        {
            ConnectTimeout = 2
        };
        using var testConn = new Microsoft.Data.SqlClient.SqlConnection(testBuilder.ConnectionString);
        testConn.Open();
        isSqlServerOnline = true;
    }
    catch
    {
        isSqlServerOnline = false;
    }

    if (isSqlServerOnline)
    {
        Console.WriteLine("[DATABASE] Đã kết nối thành công tới Microsoft SQL Server.");
        builder.Services.AddDbContext<LeVietTravelDbContext>(options =>
            options.UseSqlServer(rawConnectionString));
    }
    else
    {
        Console.WriteLine("[DATABASE] Không kết nối được MySQL hoặc SQL Server. Tự động dùng In-Memory Database để chạy ngay lập tức!");
        builder.Services.AddDbContext<LeVietTravelDbContext>(options =>
            options.UseInMemoryDatabase("LeVietTravel_DevDb"));
    }
}

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
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<LeVietTravelDbContext>();
        DbInitializer.Initialize(context);
    }
    catch (Exception ex)
    {
        Console.WriteLine("[WARNING] Không thể seed dữ liệu mẫu: " + ex.Message);
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
        RequestPath = "",
        OnPrepareResponse = ctx =>
        {
            if (ctx.File.Name.EndsWith(".html", StringComparison.OrdinalIgnoreCase))
            {
                ctx.Context.Response.Headers.Append("Cache-Control", "no-cache, no-store, must-revalidate");
                ctx.Context.Response.Headers.Append("Pragma", "no-cache");
                ctx.Context.Response.Headers.Append("Expires", "0");
            }
        }
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

// SPA Fallback cho React Router (cả Client và Admin)
if (Directory.Exists(frontendPath))
{
    var fileProvider = new PhysicalFileProvider(frontendPath);
    app.MapFallbackToFile("admin/{*path:nonfile}", "admin/index.html", new StaticFileOptions
    {
        FileProvider = fileProvider,
        OnPrepareResponse = ctx =>
        {
            ctx.Context.Response.Headers.Append("Cache-Control", "no-cache, no-store, must-revalidate");
        }
    });
    app.MapFallbackToFile("index.html", new StaticFileOptions
    {
        FileProvider = fileProvider,
        OnPrepareResponse = ctx =>
        {
            ctx.Context.Response.Headers.Append("Cache-Control", "no-cache, no-store, must-revalidate");
        }
    });
}
else
{
    app.MapFallbackToFile("index.html");
}

Console.WriteLine("====================================================================");
Console.WriteLine("  LEVIET TRAVEL API ĐANG CHẠY TẠI: http://localhost:5000");
Console.WriteLine("  TÀI LIỆU SWAGGER:               http://localhost:5000/swagger");
Console.WriteLine("  WEBSITE KHÁCH HÀNG:             http://localhost:5000");
Console.WriteLine("====================================================================");

app.Run();
