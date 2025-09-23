using EduZasAPI.Domain.Common;

namespace EduZasAPI.Application.Common;

/// <summary>
/// Interfaz genérica que agrupa operaciones CRUD y de consulta para un repositorio asincrónico.
/// </summary>
/// <typeparam name="I">Tipo del identificador de la entidad.</typeparam>
/// <typeparam name="E">Tipo de la entidad que será gestionada, debe implementar <see cref="IIdentifiable{I}"/>.</typeparam>
/// <typeparam name="NE">Tipo de los datos necesarios para crear la entidad.</typeparam>
/// <typeparam name="UE">Tipo de los datos necesarios para actualizar la entidad.</typeparam>
/// <typeparam name="C">Tipo de criterios de consulta, debe implementar <see cref="ICriteriaDTO"/>.</typeparam>
public interface IRepositoryAsync<I, E, NE, UE, C>
    : ICreatorAsync<E, NE>, IUpdaterAsync<E, UE>, IReaderAsync<I, E>, IDeleterAsync<I, E>, IQuerierAsync<E, C>
    where I : notnull
    where E : notnull, IIdentifiable<I>
    where NE : notnull
    where UE : notnull
    where C : notnull, ICriteriaDTO
{ }
