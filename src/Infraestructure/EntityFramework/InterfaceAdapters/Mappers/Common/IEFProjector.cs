using System.Linq.Expressions;
using Application.DTOs.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.Common;

/// <summary>
/// Extiende la funcionalidad de IMapper para proporcionar una expresión de proyección
/// que puede ser traducida a SQL por un proveedor LINQ como Entity Framework.
/// </summary>
/// <remarks>
/// Esta interfaz combina la capacidad de mapeo en memoria (a través de IMapper) con
/// la capacidad de proyección en la base de datos, asegurando que la lógica de mapeo
/// tenga una única fuente de verdad.
/// </remarks>
/// <typeparam name="TIn">El tipo del objeto de origen (usualmente la entidad de EF).</typeparam>
/// <typeparam name="TOut">El tipo del objeto de destino (usualmente la entidad de Dominio o un DTO).</typeparam>
public interface IEFProjector<TIn, TOut, TCriteria>
    where TCriteria : CriteriaDTO
{
    /// <summary>
    /// Obtiene el árbol de expresión que define la proyección de <typeparamref name="TIn"/> a <typeparamref name="TOut"/>.
    /// Esta expresión está diseñada para ser utilizada por Entity Framework para generar consultas optimizadas.
    /// </summary>
    Expression<Func<TIn, TOut>> GetProjection(TCriteria criteria);
}
