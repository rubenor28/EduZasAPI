using Composition;
using EntityFramework.Application.DTOs;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Tests;

public static class ServiceProviderFactory
{
    public static ServiceProvider CreateServiceProvider()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var services = new ServiceCollection();

        // 1. Registrar toda la infraestructura de producción
        services.AddApplicationServices(configuration);

        // 2. Registrar los servicios específicos para tests

        // Configurar SQLite en memoria
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        services.AddSingleton(connection); // Mantener la conexión abierta

        services.AddDbContext<EduZasDotnetContext>(options => options.UseSqlite(connection));

        // 4. Construir el proveedor y crear el esquema de BD
        var serviceProvider = services.BuildServiceProvider();

        using (var scope = serviceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<EduZasDotnetContext>();
            // Asegura que la base de datos se cree en la conexión en memoria
            dbContext.Database.EnsureCreated();
        }

        return serviceProvider;
    }
}
