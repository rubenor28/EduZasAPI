using Domain.ValueObjects;

namespace InterfaceAdapters.Mappers.Common;

public static class OptionMapperClass
{
    public static T? ToNullable<T>(this Optional<T> source)
        where T : class => source.IsSome ? source.Unwrap() : null;
}

public static class OptionMapperStruct
{
    public static T? ToNullable<T>(this Optional<T> source)
        where T : struct => source.IsSome ? source.Unwrap() : null;
}
