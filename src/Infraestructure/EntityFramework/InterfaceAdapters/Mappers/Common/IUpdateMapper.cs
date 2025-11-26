namespace EntityFramework.InterfaceAdapters.Mappers.Common;

public interface IUpdateMapper<in TSource, in TDestination>
{
    void Map(TSource source, TDestination destination);
}
