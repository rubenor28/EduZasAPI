namespace InterfaceAdapters.Mappers.Common;

public interface IBidirectionalMapper<TIn, TOut> : IMapper<TIn, TOut>
{
    public TIn MapFrom(TOut input);
}

public interface IBidirectionalMapper<T1, T2, TOut> : IMapper<T1, T2, TOut>
{
    public (T1, T2) MapFrom(TOut input);
}

public interface IBidirectionalMapper<T1, T2, T3, TOut> : IMapper<T1, T2, T3, TOut>
{
    public (T1, T2, T2) MapFrom(TOut input);
}
