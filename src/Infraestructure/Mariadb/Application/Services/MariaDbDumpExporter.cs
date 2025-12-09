using System.Diagnostics;
using System.Text;
using Application.Services;
using MySql.Data.MySqlClient;

namespace Mariadb.Application.Services;

/// <summary>
/// Exportador de base de datos usando `mariadb-dump`.
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
    /// Inicializa usando una cadena de conexión.
    /// </summary>
    /// <param name="connectionString">Cadena de conexión.</param>
    /// <param name="dumpPath">Ruta al ejecutable `mariadb-dump`.</param>
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
    /// Inicializa con parámetros explícitos.
    /// </summary>
    /// <param name="user">Usuario.</param>
    /// <param name="password">Contraseña.</param>
    /// <param name="database">Base de datos.</param>
    /// <param name="host">Host (default: localhost).</param>
    /// <param name="dumpPath">Ruta ejecutable (default: mariadb-dump).</param>
    /// <param name="port">Puerto (default: 3306).</param>
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
    /// Exporta el respaldo de la base de datos.
    /// </summary>
    /// <returns>Stream con el contenido del respaldo.</returns>
    /// <exception cref="InvalidOperationException">Si falla el proceso de dump.</exception>
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
