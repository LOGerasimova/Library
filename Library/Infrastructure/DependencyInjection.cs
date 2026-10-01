using Library.Application.Common.Interfaces;
using Library.Domain.Interfaces;
using Library.Infrastructure.Persistence;
using Library.Infrastructure.Persistence.Repositories;
using Library.Infrastructure.Xml;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Library.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // Persistence (PostgreSQL)
        services.AddOptions<PostgresSettings>().BindConfiguration(PostgresSettings.SectionName);
        services.AddDbContext<LibraryDbContext>((sp, options) =>
        {
            var settings = sp.GetRequiredService<IOptions<PostgresSettings>>().Value;
            var connectionString = $"Host={settings.Host};Port={settings.Port};Database={settings.Database};Username={settings.User};Password={settings.Password};Trust Server Certificate={settings.TrustServerCertificate};";
            options.UseNpgsql(connectionString);
        });

        // Repositories
        services.AddScoped<IBookRepository, BookRepository>();

        // Services
        services.AddScoped<IXmlTocConverter, XmlTocConverter>();

        return services;
    }
}
