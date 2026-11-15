using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Cấu hình YARP Reverse Proxy chuyển tiếp sang LeViet_Travel_API (:5001)
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

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

app.UseCors("AllowAll");

// Cấu hình phục vụ giao diện Web Frontend từ thư mục Frontend
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

// Chuyển tiếp các request /api/... sang backend API
app.MapReverseProxy();

Console.WriteLine("====================================================================");
Console.WriteLine("  API GATEWAY ĐANG CHẠY TẠI:     http://localhost:5000");
Console.WriteLine("  WEBSITE KHÁCH HÀNG:            http://localhost:5000");
Console.WriteLine("  WEBSITE QUẢN TRỊ VIÊN:         http://localhost:5000/admin.html");
Console.WriteLine("====================================================================");

app.Run();
