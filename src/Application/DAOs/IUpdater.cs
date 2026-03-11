namespace Application.DAOs;

/// <summary>
/// Interfaz genérica para actualizar entidades en un repositorio o DAO.
/// </summary>
/// <typeparam name="E">Tipo de la entidad que será actualizada.</typeparam>
/// <typeparam name="UE">Tipo de los datos usados para actualizar la entidad.</typeparam>
public interface IUpdaterAsync<E, UE>
    where E : notnull
    where UE : notnull
{
    /// <summary>
    /// Actualiza una entidad usando los datos proporcionados.
    /// </summary>
    /// <param name="updateData">Datos necesarios para la actualización de la entidad.</param>
    /// <returns>La entidad actualizada de tipo <typeparamref name="E"/>.</returns>
    Task<E> UpdateAsync(UE updateData);

    /// <summary>
    /// Actualiza de forma masiva una colección de entidades usando los datos proporcionados.
    /// </summary>
    /// <param name="updates">Colección de objetos de datos necesarios para la actualización de las entidades.</param>
    /// <returns>
    /// Una tarea que representa la operación asíncrona. El resultado de la tarea es una colección
    /// de las entidades actualizadas de tipo <typeparamref name="E"/>.
    /// </returns>
    Task<IEnumerable<E>> BulkUpdateAsync(IEnumerable<UE> updates);
}
