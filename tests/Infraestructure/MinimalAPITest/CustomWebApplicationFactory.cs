
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using EntityFramework.Application.DTOs;
using Microsoft.Data.Sqlite;
using System.Data.Common;
using System.IO;
using DotNetEnv;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace MinimalAPITest;

/// <summary>
/// Una WebApplicationFactory personalizada para las pruebas de integración.
/// Lee la configuración desde el archivo .env del proyecto y sobrescribe la
/// base de datos para usar SQLite en memoria.
/// </summary>
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, configBuilder) =>
        {
            configBuilder.Sources.Clear();

            // Calcular la ruta al archivo .env en la raíz del proyecto EduZasAPI
            var contentRoot = Directory.GetCurrentDirectory();
            var envPath = Path.Combine(contentRoot, "..", "..", "..", ".env");
            
            // Cargar el archivo .env en un diccionario
            var envConfig = new Dictionary<string, string>();
            if (File.Exists(envPath))
            {
                foreach (var line in File.ReadAllLines(envPath))
                {
                    var parts = line.Split('=', 2);
                    if (parts.Length == 2 && !string.IsNullOrWhiteSpace(parts[0]))
                    {
                        // La clave ya está en el formato correcto (ej: "ServerOptions__FrontEndUrl")
                        var key = parts[0];
                        envConfig[key] = parts[1];
                    }
                }
            }

            // Añadir el diccionario como una fuente de configuración en memoria.
            // Esto es más robusto y directo que depender de variables de proceso.
            configBuilder.AddInMemoryCollection(envConfig);
        });


        builder.ConfigureServices(services =>
        {
            // 1. Eliminar la configuración del DbContext de producción
            var dbContextOptionsDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<EduZasDotnetContext>));

            if (dbContextOptionsDescriptor != null)
            {
                services.Remove(dbContextOptionsDescriptor);
            }
            
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(EduZasDotnetContext));

            if (dbContextDescriptor != null)
            {
                services.Remove(dbContextDescriptor);
            }

            // 2. Registrar una conexión a SQLite en memoria que se mantendrá viva
            services.AddSingleton<DbConnection>(container =>
            {
                var connection = new SqliteConnection("DataSource=:memory:;Cache=Shared");
                connection.Open();
                return connection;
            });

            // 3. Registrar el DbContext para que use la conexión SQLite en memoria
            services.AddDbContext<EduZasDotnetContext>((container, options) =>
            {
                var connection = container.GetRequiredService<DbConnection>();
                options.UseSqlite(connection);
            });
        });
    }
}
