using LeVietTravel.Common.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Cấu hình YARP Reverse Proxy
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// CORS cho phép các client bên ngoài gọi vào nếu cần
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

// Custom Middlewares
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseCors("AllowAll");

// Cho phép phục vụ giao diện tĩnh HTML, CSS, JavaScript từ thư mục wwwroot
app.UseDefaultFiles();
app.UseStaticFiles();

// Định tuyến YARP sang các Microservices
app.MapReverseProxy();

app.Run();
