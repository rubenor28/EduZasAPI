using Domain.Extensions;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;

public class ClassTestUpdateMapper : IUpdateMapper<ClassTestUpdateDTO, TestPerClass>
{
    public void Map(ClassTestUpdateDTO s, TestPerClass d)
    {
        s.AllowModifyAnswers.IfSome(allow => d.AllowModifyAnswers = allow);
    }
}
