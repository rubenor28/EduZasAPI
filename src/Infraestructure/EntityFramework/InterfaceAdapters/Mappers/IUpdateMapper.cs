namespace EntityFramework.InterfaceAdapters.Mappers;

public interface IUpdateMapper<in TSource, in TDestination>
{
    void Map(TSource source, TDestination destination);
}
