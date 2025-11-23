using Domain.Enums;
using Domain.ValueObjects;
using InterfaceAdapters.Mappers.Common;

namespace InterfaceAdapters.Mappers.Users;

public class UserTypeIntMapper
    : IBidirectionalResultMapper<int, UserType, Unit>,
        IMapper<UserType, int>
{
    public Result<UserType, Unit> Map(int input) =>
        input switch
        {
            0 => UserType.STUDENT,
            1 => UserType.PROFESSOR,
            2 => UserType.ADMIN,
            _ => Unit.Value,
        };

    public int Map(UserType input) => MapFrom(input);

    public int MapFrom(UserType input) =>
        input switch
        {
            UserType.STUDENT => 0,
            UserType.PROFESSOR => 1,
            UserType.ADMIN => 2,
            _ => throw new NotImplementedException(),
        };
}

public class UserTypeUintMapper
    : IBidirectionalResultMapper<uint, UserType, Unit>,
        IMapper<UserType, uint>
{
    public Result<UserType, Unit> Map(uint input) =>
        input switch
        {
            0 => UserType.STUDENT,
            1 => UserType.PROFESSOR,
            2 => UserType.ADMIN,
            _ => Unit.Value,
        };

    public uint Map(UserType input) => MapFrom(input);

    public uint MapFrom(UserType input) =>
        input switch
        {
            UserType.STUDENT => 0,
            UserType.PROFESSOR => 1,
            UserType.ADMIN => 2,
            _ => throw new NotImplementedException(),
        };
}

public class UserTypeLongMapper
    : IBidirectionalResultMapper<long, UserType, Unit>,
        IMapper<UserType, long>
{
    public Result<UserType, Unit> Map(long input) =>
        input switch
        {
            0 => UserType.STUDENT,
            1 => UserType.PROFESSOR,
            2 => UserType.ADMIN,
            _ => Unit.Value,
        };

    public long Map(UserType input) => MapFrom(input);

    public long MapFrom(UserType input) =>
        input switch
        {
            UserType.STUDENT => 0,
            UserType.PROFESSOR => 1,
            UserType.ADMIN => 2,
            _ => throw new NotImplementedException(),
        };
}

public class UserTypeUlongMapper
    : IBidirectionalResultMapper<ulong, UserType, Unit>,
        IMapper<UserType, ulong>
{
    public Result<UserType, Unit> Map(ulong input) =>
        input switch
        {
            0 => UserType.STUDENT,
            1 => UserType.PROFESSOR,
            2 => UserType.ADMIN,
            _ => Unit.Value,
        };

    public ulong Map(UserType input) => MapFrom(input);

    public ulong MapFrom(UserType input) =>
        input switch
        {
            UserType.STUDENT => 0,
            UserType.PROFESSOR => 1,
            UserType.ADMIN => 2,
            _ => throw new NotImplementedException(),
        };
}

public class UserTypeStringMapper
    : IBidirectionalResultMapper<string, UserType, Unit>,
        IMapper<UserType, string>
{
    public Result<UserType, Unit> Map(string input) =>
        input switch
        {
            "STUDENT" => UserType.STUDENT,
            "PROFESSOR" => UserType.PROFESSOR,
            "ADMIN" => UserType.ADMIN,
            _ => Unit.Value,
        };

    public string Map(UserType input) => MapFrom(input);

    public string MapFrom(UserType input) =>
        input switch
        {
            UserType.STUDENT => "STUDENT",
            UserType.PROFESSOR => "PROFESSOR",
            UserType.ADMIN => "ADMIN",
            _ => throw new NotImplementedException(),
        };
}

public class OptionalUserTypeUintMapper
    : IBidirectionalResultMapper<uint?, Optional<UserType>, Unit>,
        IMapper<Optional<UserType>, uint?>
{
    private readonly IBidirectionalResultMapper<uint, UserType, Unit> _mapper;

    public OptionalUserTypeUintMapper(IBidirectionalResultMapper<uint, UserType, Unit> mapper)
    {
        _mapper = mapper;
    }

    public Result<Optional<UserType>, Unit> Map(uint? input)
    {
        if (input is null)
        {
            return Optional<UserType>.None();
        }

        return _mapper.Map(input.Value).Match<Result<Optional<UserType>, Unit>>(
            ok => Optional<UserType>.Some(ok),
            err => err
        );
    }

    public uint? Map(Optional<UserType> input) => MapFrom(input);

    public uint? MapFrom(Optional<UserType> input) =>
        input.Match<uint?>(
            some => _mapper.MapFrom(some),
            () => null
        );
}
