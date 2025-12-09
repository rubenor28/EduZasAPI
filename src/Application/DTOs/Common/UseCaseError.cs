using System.Collections.Generic;

namespace Application.DTOs.Common;

/// <summary>
/// Representa el tipo base abstracto para un error de un caso de uso.
/// Sirve como contrato común para los diferentes tipos de errores que una operación puede devolver.
/// Las variantes se instancian mediante las fábricas estáticas en <see cref="UseCaseError"/>
/// </summary>
public abstract record UseCaseError;

/// <summary>
/// Proporciona métodos de fábrica estáticos para crear instancias de los diferentes tipos de error de los casos de uso.
/// </summary>
public class UseCaseErrors
{
    /// <summary>
    /// Instancia única y privada del error de tipo Unauthorized para ser reutilizada (Patrón Singleton).
    /// </summary>
    private static readonly UnauthorizedError _unauthorized = new();
    private static readonly NotFoundError _notFound = new();

    /// <summary>
    /// Crea un error que representa una validación de entrada de datos fallida.
    /// </summary>
    /// <param name="errors">La lista de errores de campo específicos.</param>
    /// <returns>Una instancia de error de tipo <see cref="Input"/>.</returns>
    public static UseCaseError Input(IEnumerable<FieldErrorDTO> Errors) => new InputError(Errors);

    /// <summary>
    /// Devuelve la instancia única de un error que representa una falla de autorización.
    /// </summary>
    /// <returns>La instancia singleton del error de tipo <see cref="Unauthorized"/>.</returns>
    public static UseCaseError Unauthorized() => _unauthorized;

    /// <summary>
    /// Devuelve una instancia única del error de tipo <see cref="NotFound"/>
    /// </summary>
    public static UseCaseError NotFound() => _notFound;

    /// <summary>
    /// Crea un error de conflicto, indicando que la operación no pudo completarse debido al estado actual del recurso.
    /// </summary>
    /// <param name="msg">Mensaje descriptivo del conflicto.</param>
    /// <returns>Una instancia de error de tipo <see cref="Conflict"/>.</returns>
    public static Conflict Conflict(string msg) => new(msg);
}

/// <summary>
/// Representa un error ocurrido debido a una validación de entrada de datos fallida.
/// </summary>
/// <param name="errors">Conjunto de errores que detallan los campos y mensajes de la validación.</param>
public sealed record InputError(IEnumerable<FieldErrorDTO> Errors) : UseCaseError;

/// <summary>
/// Representa un error ocurrido porque el usuario no tiene los permisos necesarios para realizar una operación.
/// </summary>
public sealed record UnauthorizedError() : UseCaseError;

/// <summary>
/// Representa un error ocurrido cuando no se encuentra un recurso.
/// </summary>
public sealed record NotFoundError() : UseCaseError;

/// <summary>
/// Representa un error de conflicto de estado (ej. duplicados, violación de reglas de negocio).
/// </summary>
/// <param name="Message">Mensaje descriptivo del conflicto.</param>
public sealed record Conflict(string Message) : UseCaseError;
