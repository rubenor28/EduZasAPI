namespace Application.DAOs;

/// <summary>
/// Define una interfaz genérica para la creación de una única entidad en la base de datos.
/// </summary>
/// <typeparam name="E">Tipo de la entidad que será creada.</typeparam>
/// <typeparam name="NE">Tipo de los datos necesarios para crear la entidad.</typeparam>
public interface ICreatorAsync<E, NE>
    where E : notnull
    where NE : notnull
{
    /// <summary>
    /// Crea y persiste una nueva entidad basada en los datos proporcionados.
    /// </summary>
    /// <param name="data">Objeto de datos necesario para la creación de la entidad.</param>
    /// <returns>
    /// Una tarea que representa la operación asíncrona. El resultado de la tarea es la entidad
    /// creada de tipo <typeparamref name="E"/>.
    /// </returns>
    Task<E> AddAsync(NE data);
}
