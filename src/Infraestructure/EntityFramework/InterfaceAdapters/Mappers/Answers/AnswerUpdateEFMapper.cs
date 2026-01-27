using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;

public class AnswerUpdateEFMapper : IUpdateMapper<AnswerUpdateDTO, Answer>
{
    public void Map(AnswerUpdateDTO s, Answer d)
    {
        d.TryFinished = s.TryFinished ?? d.TryFinished;
        d.Content = s.Content ?? d.Content;
        d.Metadata = s.Metadata ?? d.Metadata;

        Console.WriteLine($"Try Finished Source: {s.TryFinished}, final: {s.TryFinished}");
    }
}
