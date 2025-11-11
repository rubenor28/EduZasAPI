using Domain.ValueObjects;

namespace Domain.Entities;

public sealed record ClassProfessorDomain : IIdentifiable<UserClassRelationId>
{
    public required UserClassRelationId Id { get; set; }
    public required bool IsOwner { get; set; }
    public required DateTime CreatedAt { get; set; }
}
