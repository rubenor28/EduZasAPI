using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.Answers;

public sealed class AnswerStudentUpdateEFMapper : IUpdateMapper<AnswerUpdateStudentDTO, Answer>
{
    public void Map(AnswerUpdateStudentDTO source, Answer destination)
    {
        destination.Content = source.Content;
    }
}
