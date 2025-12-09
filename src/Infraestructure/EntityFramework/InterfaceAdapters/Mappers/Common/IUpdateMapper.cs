namespace EntityFramework.InterfaceAdapters.Mappers.Common;

/// <summary>
/// Interfaz para mapear actualizaciones de una fuente a un destino existente.
/// </summary>
/// <typeparam name="TSource">Tipo de origen.</typeparam>
/// <typeparam name="TDestination">Tipo de destino.</typeparam>
public interface IUpdateMapper<in TSource, in TDestination>
{
    /// <summary>
    /// Mapea las propiedades de la fuente al destino.
    /// </summary>
    /// <param name="source">Objeto fuente.</param>
    /// <param name="destination">Objeto destino a actualizar.</param>
    void Map(TSource source, TDestination destination);
}
