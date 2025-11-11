using Domain.ValueObjects;

namespace Domain.Entities;

public sealed record ClassStudentDomain : IIdentifiable<UserClassRelationId>
{
    public required UserClassRelationId Id { get; set; }
    public required bool Hidden { get; set; }
    public required DateTime CreatedAt { get; set; }
}
