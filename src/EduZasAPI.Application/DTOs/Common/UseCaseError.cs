namespace EduZasAPI.Application.Common;

/// <summary>
/// Representa el tipo base abstracto para un error de un caso de uso.
/// Sirve como contrato común para los diferentes tipos de errores que una operación puede devolver.
/// Las variantes se instancian mediante las fábricas estáticas en <see cref="UseCaseError"/>
/// </summary>
public abstract record UseCaseErrorImpl;

/// <summary>
/// Proporciona métodos de fábrica estáticos para crear instancias de los diferentes tipos de error de los casos de uso.
/// </summary>
public class UseCaseError
{
    /// <summary>
    /// Instancia única y privada del error de tipo Unauthorized para ser reutilizada (Patrón Singleton).
    /// </summary>
    private static readonly Unauthorized _unauthorized = new();

    /// <summary>
    /// Crea un error que representa una validación de entrada de datos fallida.
    /// </summary>
    /// <param name="errors">La lista de errores de campo específicos.</param>
    /// <returns>Una instancia de error de tipo <see cref="Input"/>.</returns>
    public static UseCaseErrorImpl InputError(List<FieldErrorDTO> errors) => new Input(errors);

    /// <summary>
    /// Devuelve la instancia única de un error que representa una falla de autorización.
    /// </summary>
    /// <returns>La instancia singleton del error de tipo <see cref="Unauthorized"/>.</returns>
    public static UseCaseErrorImpl UnauthorizedError() => _unauthorized;
}

/// <summary>
/// Representa un error ocurrido debido a una validación de entrada de datos fallida.
/// </summary>
/// <param name="errors">Lista de errores que detallan los campos y mensajes de la validación.</param>
public record Input(List<FieldErrorDTO> errors) : UseCaseErrorImpl;

/// <summary>
/// Representa un error ocurrido porque el usuario no tiene los permisos necesarios para realizar una operación.
/// </summary>
public record Unauthorized() : UseCaseErrorImpl;
