using EduZasAPI.Domain.Common;

namespace EduZasAPI.Application.Common;

/// <summary>
/// Interfaz genérica para leer o recuperar entidades de un repositorio por su identificador.
/// </summary>
/// <typeparam name="I">Tipo del identificador de la entidad.</typeparam>
/// <typeparam name="E">Tipo de la entidad a recuperar, debe implementar <see cref="IIdentifiable{I}"/>.</typeparam>
public interface IReaderAsync<I, E>
    where I : notnull
    where E : notnull, IIdentifiable<I>
{
    /// <summary>
    /// Recupera una entidad por su identificador.
    /// </summary>
    /// <param name="id">Identificador de la entidad.</param>
    /// <returns>
    /// Un <see cref="Optional{E}"/> que contiene la entidad si se encuentra,
    /// o vacío si no existe.
    /// </returns>
    Task<Optional<E>> GetAsync(I id);
}
