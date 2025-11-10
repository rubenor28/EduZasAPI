using Domain.Enums;
using Domain.ValueObjects;
using InterfaceAdapters.Mappers.Common;

namespace InterfaceAdapters.Mappers.Users;

public class UserTypeMapper
    : IMapper<UserType, int>,
        IMapper<int, Result<UserType, Unit>>,
        IMapper<UserType, long>,
        IMapper<long, Result<UserType, Unit>>,
        IMapper<UserType, ulong>,
        IMapper<ulong, Result<UserType, Unit>>,
        IMapper<UserType, string>,
        IMapper<string, Result<UserType, Unit>>,
        IMapper<uint, Result<UserType, Unit>>,
        IMapper<UserType, uint>
{
    Result<UserType, Unit> IMapper<uint, Result<UserType, Unit>>.Map(uint input) =>
        input switch
        {
            0 => UserType.STUDENT,
            1 => UserType.PROFESSOR,
            2 => UserType.ADMIN,
            _ => Unit.Value,
        };

    uint IMapper<UserType, uint>.Map(UserType input) =>
        input switch
        {
            UserType.STUDENT => 0,
            UserType.PROFESSOR => 1,
            UserType.ADMIN => 2,
            _ => throw new NotImplementedException(),
        };

    int IMapper<UserType, int>.Map(UserType input) =>
        input switch
        {
            UserType.STUDENT => 0,
            UserType.PROFESSOR => 1,
            UserType.ADMIN => 2,
            _ => throw new NotImplementedException(),
        };

    Result<UserType, Unit> IMapper<int, Result<UserType, Unit>>.Map(int input) =>
        input switch
        {
            0 => UserType.STUDENT,
            1 => UserType.PROFESSOR,
            2 => UserType.ADMIN,
            _ => Unit.Value,
        };

    long IMapper<UserType, long>.Map(UserType input) =>
        input switch
        {
            UserType.STUDENT => 0,
            UserType.PROFESSOR => 1,
            UserType.ADMIN => 2,
            _ => throw new NotImplementedException(),
        };

    Result<UserType, Unit> IMapper<long, Result<UserType, Unit>>.Map(long input) =>
        input switch
        {
            0 => UserType.STUDENT,
            1 => UserType.PROFESSOR,
            2 => UserType.ADMIN,
            _ => Unit.Value,
        };

    ulong IMapper<UserType, ulong>.Map(UserType input) =>
        input switch
        {
            UserType.STUDENT => 0,
            UserType.PROFESSOR => 1,
            UserType.ADMIN => 2,
            _ => throw new NotImplementedException(),
        };

    Result<UserType, Unit> IMapper<ulong, Result<UserType, Unit>>.Map(ulong input) =>
        input switch
        {
            0 => UserType.STUDENT,
            1 => UserType.PROFESSOR,
            2 => UserType.ADMIN,
            _ => Unit.Value,
        };

    string IMapper<UserType, string>.Map(UserType input) =>
        input switch
        {
            UserType.STUDENT => "STUDENT",
            UserType.PROFESSOR => "PROFESSOR",
            UserType.ADMIN => "ADMIN",
            _ => throw new NotImplementedException(),
        };

    Result<UserType, Unit> IMapper<string, Result<UserType, Unit>>.Map(string input) =>
        input switch
        {
            "STUDENT" => UserType.STUDENT,
            "PROFESSOR" => UserType.PROFESSOR,
            "ADMIN" => UserType.ADMIN,
            _ => Unit.Value,
        };
}
