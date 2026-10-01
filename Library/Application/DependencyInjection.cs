using Library.Application.Books;

namespace Library.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IBookAppService, BookAppService>();
        return services;
    }
}

