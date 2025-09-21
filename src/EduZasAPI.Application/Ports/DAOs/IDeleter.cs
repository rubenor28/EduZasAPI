namespace EduZasAPI.Application.Ports.DAOs;

using EduZasAPI.Domain.ValueObjects.Common;

/// <summary>
/// Interfaz genérica para eliminar entidades de un repositorio o DAO.
/// </summary>
/// <typeparam name="I">Tipo del identificador de la entidad.</typeparam>
/// <typeparam name="E">Tipo de la entidad que se va a eliminar, debe implementar <see cref="IIdentifiable{I}"/>.</typeparam>
public interface IDeleterAsync<I, E>
    where I : notnull
    where E : notnull, IIdentifiable<I>
{
    /// <summary>
    /// Elimina la entidad identificada por el ID proporcionado.
    /// </summary>
    /// <param name="id">Identificador de la entidad a eliminar.</param>
    /// <returns>
    /// Un <see cref="Optional{E}"/> que contiene la entidad eliminada si existía,
    /// o vacío si no se encontró.
    /// </returns>
    Task<Optional<E>> DeleteAsync(I id);
}
