using System.Diagnostics;
using System.Text;
using Application.Services;
using MySql.Data.MySqlClient;

namespace Mariadb.Application.Services;

/// <summary>
/// Importador de base de datos usando `mysql` (o `mariadb`).
/// </summary>
public sealed class MariaDbImporter : IDatabaseImporter
{
    private readonly string _user;
    private readonly string _password;
    private readonly string _database;
    private readonly string _host;
    private readonly string _mysqlPath;
    private readonly uint _port;

    /// <summary>
    /// Inicializa usando una cadena de conexión.
    /// </summary>
    /// <param name="connectionString">Cadena de conexión.</param>
    /// <param name="mysqlPath">Ruta al ejecutable `mysql`.</param>
    public MariaDbImporter(string connectionString, string mysqlPath)
    {
        _mysqlPath = mysqlPath;

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
    /// <param name="mysqlPath">Ruta ejecutable (default: mysql).</param>
    /// <param name="port">Puerto (default: 3306).</param>
    public MariaDbImporter(
        string user,
        string password,
        string database,
        string host = "localhost",
        string mysqlPath = "mysql",
        uint port = 3306
    )
    {
        _user = user;
        _password = password;
        _database = database;
        _host = host;
        _mysqlPath = mysqlPath;
        _port = port;
    }

    /// <summary>
    /// Restaura la base de datos desde un stream.
    /// </summary>
    /// <param name="input">Stream con el archivo SQL.</param>
    /// <returns>Tarea asíncrona.</returns>
    /// <exception cref="InvalidOperationException">Si falla el proceso de importación.</exception>
    public async Task RestoreAsync(Stream input)
    {
        var args = new StringBuilder();
        args.Append($"-h {_host} -u {_user} ");
        args.Append($"--port={_port} ");
        args.Append($"--password={_password} "); // WARN: la contraseña queda visible en ps/top.
        args.Append($"{_database}");

        var psi = new ProcessStartInfo
        {
            FileName = _mysqlPath,
            Arguments = args.ToString(),
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        using var process =
            Process.Start(psi) ?? throw new InvalidOperationException("No se pudo iniciar mysql.");

        // Escribir el contenido del stream a la entrada estándar del proceso
        await input.CopyToAsync(process.StandardInput.BaseStream);
        await process.StandardInput.BaseStream.FlushAsync();
        process.StandardInput.Close();

        // Esperar a que el proceso termine
        await process.WaitForExitAsync();
        if (process.ExitCode != 0)
        {
            string error = await process.StandardError.ReadToEndAsync();
            throw new InvalidOperationException(
                $"mysql falló con código {process.ExitCode}: {error}"
            );
        }
    }
}
