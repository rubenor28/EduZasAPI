using EduZasAPI.Domain.Users;

namespace EduZasAPI.Infraestructure.MinimalAPI.Application.Common;

public class AuthPayload
{
    public required ulong Id { get; init; }
    public required UserType Role { get; set; }
}
