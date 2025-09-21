namespace EduZasAPI.Application.Ports.DAOs;

using EduZasAPI.Application.DTOs.Common;
using EduZasAPI.Domain.ValueObjects.Common;

/// <summary>
/// Define una interfaz genérica para operaciones de repositorio asíncronas.
/// </summary>
/// <typeparam name="I">Tipo del identificador de entidad. Debe ser no nulo.</typeparam>
/// <typeparam name="E">Tipo de la entidad. Debe ser no nulo e implementar <see cref="IIdentifiable{I}"/>.</typeparam>
/// <typeparam name="NE">Tipo del DTO para crear nuevas entidades. Debe ser no nulo.</typeparam>
/// <typeparam name="UE">Tipo del DTO para actualizar entidades. Debe ser no nulo.</typeparam>
/// <typeparam name="C">Tipo de los criterios de búsqueda. Debe ser no nulo e implementar <see cref="ICriteriaDTO"/>.</typeparam>
/// <remarks>
/// Esta interfaz proporciona un contrato para implementaciones de repositorio que realizan
/// operaciones CRUD asíncronas con soporte para consultas paginadas y criterios de búsqueda.
/// </remarks>
public interface IRepositoryAsync<I, E, NE, UE, C>
where I : notnull
where E : notnull, IIdentifiable<I>
where NE : notnull
where C : notnull, ICriteriaDTO
{
    /// <summary>
    /// Agrega una nueva entidad al repositorio.
    /// </summary>
    /// <param name="newData">DTO con los datos para crear la nueva entidad.</param>
    /// <returns>Una tarea que representa la operación asíncrona. El resultado contiene la entidad creada.</returns>
    public Task<E> AddAsync(NE newData);

    /// <summary>
    /// Actualiza una entidad existente en el repositorio.
    /// </summary>
    /// <param name="updateData">DTO con los datos actualizados de la entidad.</param>
    /// <returns>Una tarea que representa la operación asíncrona. El resultado contiene la entidad actualizada.</returns>
    public Task<E> UpdateAsync(UE updateData);

    /// <summary>
    /// Obtiene una entidad por su identificador.
    /// </summary>
    /// <param name="id">Identificador de la entidad a buscar.</param>
    /// <returns>
    /// Una tarea que representa la operación asíncrona. El resultado contiene un Optional 
    /// con la entidad si fue encontrada, o None si no existe.
    /// </returns>
    public Task<Optional<E>> GetAsync(I id);

    /// <summary>
    /// Elimina una entidad del repositorio por su identificador.
    /// </summary>
    /// <param name="id">Identificador de la entidad a eliminar.</param>
    /// <returns>
    /// Una tarea que representa la operación asíncrona. El resultado contiene un Optional 
    /// con la entidad eliminada si existía, o None si no existía.
    /// </returns>
    public Task<Optional<E>> DeleteAsync(I id);

    /// <summary>
    /// Obtiene entidades basadas en criterios de búsqueda específicos.
    /// </summary>
    /// <param name="query">Criterios de búsqueda para filtrar las entidades.</param>
    /// <returns>
    /// Una tarea que representa la operación asíncrona. El resultado contiene una consulta paginada
    /// con las entidades que coinciden con los criterios y metadatos de paginación.
    /// </returns>
    public Task<PaginatedQuery<E, C>> GetByAsync(C query);
}
