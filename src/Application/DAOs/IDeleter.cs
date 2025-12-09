namespace Application.DAOs;

/// <summary>
/// Interfaz genérica para eliminar entidades de un repositorio o DAO.
/// </summary>
/// <typeparam name="I">Tipo del identificador de la entidad.</typeparam>
/// <typeparam name="E">Tipo de la entidad que se va a eliminar, debe implementar <see cref="IIdentifiable{I}"/>.</typeparam>
public interface IDeleterAsync<I, E>
    where I : notnull
    where E : notnull
{
    /// <summary>
    /// Elimina la entidad identificada por el ID proporcionado.
    /// </summary>
    /// <param name="id">Identificador de la entidad a eliminar.</param>
    /// <returns>
    /// La entidad eliminada
    /// </returns>
    Task<E> DeleteAsync(I id);

    /// <summary>
    /// Elimina múltiples entidades por sus identificadores.
    /// </summary>
    /// <param name="ids">Colección de identificadores de las entidades a eliminar.</param>
    /// <returns>Colección de las entidades eliminadas.</returns>
    Task<IEnumerable<E>> BulkDelete(IEnumerable<I> ids);
}
