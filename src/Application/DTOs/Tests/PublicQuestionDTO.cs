using Domain.Entities.PublicQuestions;

namespace Application.DTOs.Tests;

public record PublicQuestionDTO
{
    public Guid Id { get; set; }
    public required IPublicQuestion Data { get; set; }
}
