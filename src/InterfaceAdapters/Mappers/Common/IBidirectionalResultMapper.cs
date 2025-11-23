using Domain.ValueObjects;

namespace InterfaceAdapters.Mappers.Common;

public interface IBidirectionalResultMapper<TIn, TOut, E> : IMapper<TIn, Result<TOut, E>>
    where TOut : notnull
    where E : notnull
{
    public TIn MapFrom(TOut input);
}

public interface IBidirectionalResultMapper<T1, T2, TOut, E> : IMapper<T1, T2, Result<TOut, E>>
    where TOut : notnull
    where E : notnull
{
    public (T1, T2) MapFrom(TOut input);
}

public interface IBidirectionalResultMapper<T1, T2, T3, TOut, E>
    : IMapper<T1, T2, T3, Result<TOut, E>>
    where TOut : notnull
    where E : notnull
{
    public (T1, T2, T3) MapFrom(TOut input);
}
