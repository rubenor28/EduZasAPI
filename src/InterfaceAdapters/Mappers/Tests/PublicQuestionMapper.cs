using System.Runtime.InteropServices;
using Domain.Entities.PublicQuestions;
using Domain.Entities.Questions;
using InterfaceAdapters.Mappers.Common;

namespace InterfaceAdapters.Mappers.Tests;

public sealed class PublicQuestionMapper
    : IMapper<IQuestion, IPublicQuestion>,
        IMapper<ConceptRelationQuestion, PublicConceptRelationQuestion>,
        IMapper<MultipleChoiseQuestion, PublicMultipleChoiseQuestion>,
        IMapper<MultipleSelectionQuestion, PublicMultipleSelectionQuestion>,
        IMapper<OpenQuestion, PublicOpenQuestion>,
        IMapper<OrderingQuestion, PublicOrderingQuestion>
{
    public PublicConceptRelationQuestion Map(ConceptRelationQuestion q)
    {
        var count = q.Concepts.Count;

        var columnA = new List<string>(count);
        var columnB = new List<string>(count);

        foreach (var pair in q.Concepts)
        {
            columnA.Add(pair.ConceptA);
            columnB.Add(pair.ConceptB);
        }

        Random.Shared.Shuffle(CollectionsMarshal.AsSpan(columnA));
        Random.Shared.Shuffle(CollectionsMarshal.AsSpan(columnB));

        return new()
        {
            Title = q.Title,
            ImageUrl = q.ImageUrl,
            ColumnA = columnA,
            ColumnB = columnB,
        };
    }

    public PublicMultipleChoiseQuestion Map(MultipleChoiseQuestion input)
    {
        var options = input
            .Options.Select(opt => new PublicOption { Id = opt.Key, Text = opt.Value })
            .ToList();

        Random.Shared.Shuffle(CollectionsMarshal.AsSpan(options));

        return new()
        {
            Title = input.Title,
            ImageUrl = input.ImageUrl,
            Options = options,
        };
    }

    public PublicMultipleSelectionQuestion Map(MultipleSelectionQuestion input)
    {
        var options = input
            .Options.Select(opt => new PublicOption { Id = opt.Key, Text = opt.Value })
            .ToList();

        Random.Shared.Shuffle(CollectionsMarshal.AsSpan(options));

        return new()
        {
            Title = input.Title,
            ImageUrl = input.ImageUrl,
            Options = options,
        };
    }

    public PublicOpenQuestion Map(OpenQuestion input) =>
        new() { Title = input.Title, ImageUrl = input.ImageUrl };

    public PublicOrderingQuestion Map(OrderingQuestion input)
    {
        Random.Shared.Shuffle(CollectionsMarshal.AsSpan(input.Sequence));

        return new()
        {
            Title = input.Title,
            ImageUrl = input.ImageUrl,
            Items = input.Sequence,
        };
    }

    public IPublicQuestion Map(IQuestion input) =>
        input switch
        {
            ConceptRelationQuestion q => Map(q),
            MultipleChoiseQuestion q => Map(q),
            MultipleSelectionQuestion q => Map(q),
            OpenQuestion q => Map(q),
            OrderingQuestion q => Map(q),
            _ => throw new NotSupportedException(),
        };
}
