using Application.DAOs;
using Application.UseCases.Users;
using Composition;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using DotNetEnv;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Lee una entrada de texto del usuario desde la consola.
/// </summary>
/// <param name="message">Mensaje opcional a mostrar antes de leer.</param>
/// <returns>Texto ingresado por el usuario o null si no hay entrada.</returns>
static string? ReadUserInput(string? message = null)
{
    if (!string.IsNullOrEmpty(message))
        Console.WriteLine(message);

    Console.Write("> ");
    var input = Console.ReadLine();

    if (input is null)
        Console.WriteLine("No se proporcionó entrada");

    return input;
}

/// <summary>
/// Lee una contraseña desde la consola ocultando los caracteres.
/// </summary>
/// <returns>La contraseña ingresada.</returns>
static string ReadPassword()
{
    Console.Write("> ");
    string password = "";
    ConsoleKeyInfo key;

    do
    {
        key = Console.ReadKey(true); // Lee una tecla sin mostrarla

        // Ignora teclas de función y caracteres de control no deseados
        if (
            char.IsLetterOrDigit(key.KeyChar)
            || char.IsPunctuation(key.KeyChar)
            || char.IsSymbol(key.KeyChar)
        )
        {
            password += key.KeyChar;
            Console.Write("*"); // Muestra un asterisco por cada carácter
        }
        else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
        {
            password = password[0..^1]; // Elimina el último carácter de la cadena
            Console.Write("\b \b"); // Retrocede, escribe un espacio y retrocede de nuevo para "borrar" el asterisco
        }
    }
    // Continúa hasta que se presione Enter
    while (key.Key != ConsoleKey.Enter);

    Console.WriteLine(); // Nueva línea después de la entrada de la contraseña

    if (string.IsNullOrEmpty(password))
        Console.WriteLine("No se proporcionó entrada");

    return password;
}

try
{
    var environment = Environment.GetEnvironmentVariable("ServerOptions__Environment");
    if (environment != "Production")
    {
        var solutionRoot = AppDomain.CurrentDomain.BaseDirectory;
        var envPath = Path.Combine(solutionRoot, "..", "..", "..", "..", "..", "..", ".env");
        if (File.Exists(envPath))
        {
            Env.Load(envPath);
        }
        else
        {
            Console.WriteLine(
                "Advertencia: no se encontró el archivo .env. Usando solo variables de entorno."
            );
        }
    }
    var configuration = new ConfigurationBuilder().AddEnvironmentVariables().Build();
    var sp = new ServiceCollection().AddInfrastructure(configuration).BuildServiceProvider();

    var updater = sp.GetRequiredService<UpdateUserUseCase>();
    var emailReader = sp.GetRequiredService<IReaderAsync<string, UserDomain>>();
    var repeat = true;
    var opt = string.Empty;
    do
    {
        Console.WriteLine(
            "Bienvenido al sistema de restablecimiento de contraseñas\nIngresa una opción"
        );
        Console.WriteLine("1) Ingresar el email a restablecer");
        Console.WriteLine("2) Salir");
        opt = ReadUserInput();
        Console.Clear();

        if (string.IsNullOrEmpty(opt))
            continue;

        switch (opt)
        {
            case "1":
                var email = ReadUserInput("Ingresa el email del usuario a restablecer:");

                if (string.IsNullOrEmpty(email))
                    continue;

                var user = await emailReader.GetAsync(email);

                if (user is null)
                {
                    Console.WriteLine("No se encontró el usuario");
                    continue;
                }

                Console.WriteLine("Ingresa la nueva constraseña:");
                var newPwd = ReadPassword();
                if (string.IsNullOrEmpty(newPwd))
                    continue;

                Console.WriteLine("Confirmar contraseña:");
                var pwdConfirm = ReadPassword();
                if (string.IsNullOrEmpty(pwdConfirm))
                    continue;

                if (!string.Equals(newPwd, pwdConfirm))
                {
                    Console.WriteLine("Las contraseñas no coinciden");
                    continue;
                }

                var result = await updater.ExecuteAsync(
                    new()
                    {
                        Data = new()
                        {
                            Id = user.Id,
                            Active = user.Active,
                            Email = user.Email,
                            Role = user.Role,
                            FatherLastname = user.FatherLastname,
                            FirstName = user.FirstName,
                            MidName = user.MidName,
                            MotherLastname = user.MotherLastname,
                            Password = newPwd,
                        },
                        Executor = new() { Id = 1, Role = UserType.ADMIN },
                    }
                );

                result.Match(
                    _ => Console.WriteLine("Contraseña restablecida correctamente"),
                    e =>
                    {
                        Console.WriteLine("Error al actualizar contraseña");
                        switch (e)
                        {
                            case InputError ie:
                                Console.WriteLine("Entrada inválida\n\n");
                                foreach (var error in ie.Errors)
                                    Console.WriteLine(
                                        $"Campo: {error.Field}\nError: {error.Message}\n"
                                    );
                                Console.WriteLine();
                                break;
                            default:
                                Console.WriteLine(
                                    $"Error al restablecer contraseña. Error: {e.GetType().Name}"
                                );
                                break;
                        }
                    }
                );

                break;
            case "2":
                repeat = false;
                break;
            default:
                Console.WriteLine("Opción inválida");
                break;
        }
    } while (repeat);
}
catch (Exception e)
{
    Console.WriteLine(e.Message);
    Console.WriteLine(e.StackTrace);
}
