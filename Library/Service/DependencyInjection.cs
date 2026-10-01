namespace Library.Service;

public static class DependencyInjection
{
    public static IServiceCollection AddService(this IServiceCollection services)
    {
        services.AddControllersWithViews();
        return services;
    }
}
