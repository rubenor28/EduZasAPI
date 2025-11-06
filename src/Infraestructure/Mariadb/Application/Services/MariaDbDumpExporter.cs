using System.Diagnostics;
using System.Text;
using Application.Services;
using MySql.Data.MySqlClient;

namespace Mariadb.Application.Services;

/// <summary>
/// Implementación de <see cref="IDatabaseExporter"/> que utiliza la utilidad `mariadb-dump` (o `mysqldump`)
/// para exportar una base de datos MariaDB/MySQL.
/// </summary>
public sealed class MariaDbDumpExporter : IDatabaseExporter
{
    private readonly string _user;
    private readonly string _password;
    private readonly string _database;
    private readonly string _host;
    private readonly string _dumpPath;
    private readonly uint _port;

    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="MariaDbDumpExporter"/> utilizando una cadena de conexión.
    /// </summary>
    /// <param name="connectionString">La cadena de conexión a la base de datos.</param>
    /// <param name="dumpPath">La ruta al ejecutable `mariadb-dump` o `mysqldump`.</param>
    public MariaDbDumpExporter(string connectionString, string dumpPath)
    {
        _dumpPath = dumpPath;

        var builder = new MySqlConnectionStringBuilder(connectionString);
        _user = builder.UserID;
        _password = builder.Password;
        _database = builder.Database;
        _host = builder.Server;
        _port = builder.Port;
    }

    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="MariaDbDumpExporter"/> con parámetros de conexión explícitos.
    /// </summary>
    /// <param name="user">El nombre de usuario para la conexión a la base de datos.</param>
    /// <param name="password">La contraseña para la conexión a la base de datos.</param>
    /// <param name="database">El nombre de la base de datos a exportar.</param>
    /// <param name="host">El host de la base de datos. El valor predeterminado es "localhost".</param>
    /// <param name="dumpPath">La ruta al ejecutable `mariadb-dump` o `mysqldump`. El valor predeterminado es "mariadb-dump".</param>
    /// <param name="port">El puerto de conexión a la base de datos. El valor predeterminado es 3306.</param>
    public MariaDbDumpExporter(
        string user,
        string password,
        string database,
        string host = "localhost",
        string dumpPath = "mariadb-dump",
        uint port = 3306
    )
    {
        _user = user;
        _password = password;
        _database = database;
        _host = host;
        _dumpPath = dumpPath;
        _port = port;
    }

    /// <summary>
    /// Exporta una copia de seguridad de la base de datos a un stream de forma asíncrona utilizando `mariadb-dump`.
    /// </summary>
    /// <returns>Una tarea que representa la operación y que contiene un stream para leer el respaldo.</returns>
    /// <exception cref="InvalidOperationException">Se lanza si no se puede iniciar el proceso `mariadb-dump` o si el proceso falla.</exception>
    public async Task<Stream> ExportBackupAsync()
    {
        var tempFilePath = Path.GetTempFileName();

        var args = new StringBuilder();
        args.Append($"--host={_host} ");
        args.Append($"--user={_user} ");
        args.Append($"--port={_port} ");
        args.Append($"--password={_password} ");
        args.Append("--single-transaction --routines --triggers ");
        args.Append($"{_database}");

        var psi = new ProcessStartInfo
        {
            FileName = _dumpPath,
            Arguments = args.ToString(),
            RedirectStandardOutput = true, // Redirigimos para volcar a archivo
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        using var process =
            Process.Start(psi)
            ?? throw new InvalidOperationException("No se pudo iniciar mariadb-dump.");

        // Volcamos la salida estándar del proceso directamente a un archivo temporal.
        await using (var fileStream = new FileStream(tempFilePath, FileMode.Create))
        {
            await process.StandardOutput.BaseStream.CopyToAsync(fileStream);
        }

        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
        {
            string error = await process.StandardError.ReadToEndAsync();
            // Nos aseguramos de borrar el archivo temporal si hay un error.
            File.Delete(tempFilePath);
            throw new InvalidOperationException(
                $"mariadb-dump falló con código {process.ExitCode}: {error}"
            );
        }

        // Devolvemos un nuevo stream que apunta al archivo temporal.
        // La opción FileOptions.DeleteOnClose asegura que el archivo se borre
        // automáticamente cuando el stream sea cerrado.
        return new FileStream(
            tempFilePath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            4096,
            FileOptions.DeleteOnClose | FileOptions.Asynchronous
        );
    }
}
