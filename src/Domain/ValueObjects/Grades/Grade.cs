using System.Text.Json.Serialization;
using Domain.Entities.Questions;

namespace Domain.ValueObjects.Grades;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(ConceptRelationGrade), typeDiscriminator: QuestionTypes.ConceptRelation)]
[JsonDerivedType(typeof(MultipleSelectionGrade), typeDiscriminator: QuestionTypes.MultipleSelection)]
[JsonDerivedType(typeof(MultipleChoiseGrade), typeDiscriminator: QuestionTypes.MultipleChoise)]
[JsonDerivedType(typeof(OrderingGrade), typeDiscriminator: QuestionTypes.Ordering)]
[JsonDerivedType(typeof(OpenGrade), typeDiscriminator: QuestionTypes.Open)]
public abstract record Grade
{
    public required string Title { get; init; }
    public required Guid QuestionId { get; init; }
    public abstract uint TotalPoints { get; }
    public bool? ManualGrade { get; init; } = null;
    public uint Points => ManualGrade == true ? TotalPoints : Asserts();

    public abstract uint Asserts();
}
