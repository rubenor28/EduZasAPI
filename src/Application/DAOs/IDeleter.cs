namespace Application.DAOs;

/// <summary>
/// Interfaz genérica para eliminar entidades de un repositorio o DAO.
/// </summary>
/// <typeparam name="I">Tipo del identificador único de la entidad.</typeparam>
/// <typeparam name="E">Tipo de la entidad que se va a eliminar.</typeparam>
public interface IDeleterAsync<I, E>
    where I : notnull
    where E : notnull
{
    /// <summary>
    /// Elimina de forma asíncrona la entidad identificada por el ID proporcionado.
    /// </summary>
    /// <param name="id">Identificador único de la entidad a eliminar.</param>
    /// <returns>
    /// Una tarea que representa la operación asíncrona. El resultado de la tarea es la entidad
    /// eliminada de tipo <typeparamref name="E"/>.
    /// </returns>
    Task<E> DeleteAsync(I id);

    /// <summary>
    /// Elimina de forma asíncrona múltiples entidades por sus identificadores.
    /// </summary>
    /// <param name="ids">Colección de identificadores únicos de las entidades a eliminar.</param>
    /// <returns>
    /// Una tarea que representa la operación asíncrona. El resultado de la tarea es una colección
    /// de las entidades eliminadas de tipo <typeparamref name="E"/>.
    /// </returns>
    Task<IEnumerable<E>> BulkDelete(IEnumerable<I> ids);
}
