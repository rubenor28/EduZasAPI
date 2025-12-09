namespace Application.DAOs;

/// <summary>
/// Interfaz genérica para la creación de entidades.
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
    /// <param name="data">Datos necesarios para la creación de la entidad.</param>
    /// <returns>La entidad creada de tipo <typeparamref name="E"/>.</returns>
    Task<E> AddAsync(NE data);
}
