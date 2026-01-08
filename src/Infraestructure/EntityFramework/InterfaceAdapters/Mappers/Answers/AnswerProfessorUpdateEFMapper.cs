using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.Answers;

public sealed class AnswerProfessorUpdateEFMapper : IUpdateMapper<AnswerUpdateProfessorDTO, Answer>
{
    public void Map(AnswerUpdateProfessorDTO source, Answer destination)
    {
        destination.Metadata = source.Metadata;
    }
}
