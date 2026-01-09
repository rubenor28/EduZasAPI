using System.Runtime.InteropServices;
using Domain.Entities.PublicQuestions;
using Domain.Entities.Questions;
using InterfaceAdapters.Mappers.Common;

namespace InterfaceAdapters.Mappers.Tests;

public sealed class PublicQuestionMapper
    : IMapper<Guid, IQuestion, IPublicQuestion>,
        IMapper<Guid, ConceptRelationQuestion, PublicConceptRelationQuestion>,
        IMapper<Guid, MultipleChoiseQuestion, PublicMultipleChoiseQuestion>,
        IMapper<Guid, MultipleSelectionQuestion, PublicMultipleSelectionQuestion>,
        IMapper<Guid, OpenQuestion, PublicOpenQuestion>,
        IMapper<Guid, OrderingQuestion, PublicOrderingQuestion>
{
    public PublicConceptRelationQuestion Map(Guid id, ConceptRelationQuestion q)
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
            Id = id,
            Title = q.Title,
            ImageUrl = q.ImageUrl,
            ColumnA = columnA,
            ColumnB = columnB,
        };
    }

    public PublicMultipleChoiseQuestion Map(Guid id, MultipleChoiseQuestion input)
    {
        var options = input
            .Options.Select(opt => new PublicOption { Id = opt.Key, Text = opt.Value })
            .ToList();

        Random.Shared.Shuffle(CollectionsMarshal.AsSpan(options));

        return new()
        {
            Id = id,
            Title = input.Title,
            ImageUrl = input.ImageUrl,
            Options = options,
        };
    }

    public PublicMultipleSelectionQuestion Map(Guid id, MultipleSelectionQuestion input)
    {
        var options = input
            .Options.Select(opt => new PublicOption { Id = opt.Key, Text = opt.Value })
            .ToList();

        Random.Shared.Shuffle(CollectionsMarshal.AsSpan(options));

        return new()
        {
            Id = id,
            Title = input.Title,
            ImageUrl = input.ImageUrl,
            Options = options,
        };
    }

    public PublicOpenQuestion Map(Guid id, OpenQuestion input) =>
        new()
        {
            Id = id,
            Title = input.Title,
            ImageUrl = input.ImageUrl,
        };

    public PublicOrderingQuestion Map(Guid id, OrderingQuestion input)
    {
        Random.Shared.Shuffle(CollectionsMarshal.AsSpan(input.Sequence));

        return new()
        {
            Id = id,
            Title = input.Title,
            ImageUrl = input.ImageUrl,
            Items = input.Sequence,
        };
    }

    public IPublicQuestion Map(Guid id, IQuestion input) =>
        input switch
        {
            ConceptRelationQuestion q => Map(id, q),
            MultipleChoiseQuestion q => Map(id, q),
            MultipleSelectionQuestion q => Map(id, q),
            OpenQuestion q => Map(id, q),
            OrderingQuestion q => Map(id, q),
            _ => throw new NotSupportedException(),
        };
}
