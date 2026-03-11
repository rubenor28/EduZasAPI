namespace Application.DAOs;

/// <summary>
/// Define una interfaz genérica para la creación masiva de entidades en la base de datos.
/// </summary>
/// <typeparam name="E">Tipo de la entidad que será creada.</typeparam>
/// <typeparam name="NE">Tipo de los datos necesarios para crear la entidad.</typeparam>
public interface IBulkCreatorAsync<E, NE>
    where E : notnull
    where NE : notnull
{
    /// <summary>
    /// Crea y persiste una colección de nuevas entidades basadas en los datos proporcionados.
    /// </summary>
    /// <param name="data">Colección de objetos de datos necesarios para la creación de las entidades.</param>
    /// <returns>
    /// Una tarea que representa la operación asíncrona. El resultado de la tarea es una colección
    /// de las entidades creadas de tipo <typeparamref name="E"/>.
    /// </returns>
    Task<IEnumerable<E>> AddRangeAsync(IEnumerable<NE> data);
}
