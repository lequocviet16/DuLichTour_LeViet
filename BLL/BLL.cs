using DAL;
using Microsoft.Extensions.DependencyInjection;

namespace BLL;

public static class BLL
{
    public static IServiceCollection AddBLLServices(this IServiceCollection services)
    {
        // Đăng ký DAL
        services.AddScoped<UserDAL>();
        services.AddScoped<TourDAL>();
        services.AddScoped<BookingDAL>();

        // Đăng ký BLL
        services.AddScoped<AuthBLL>();
        services.AddScoped<TourBLL>();
        services.AddScoped<BookingBLL>();

        return services;
    }
}
