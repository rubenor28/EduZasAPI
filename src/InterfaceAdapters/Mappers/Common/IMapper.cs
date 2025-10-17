namespace InterfaceAdapters.Mappers.Common;

public interface IMapper<TInput, TOutput>
{
  public TOutput Map(TInput input);
}
