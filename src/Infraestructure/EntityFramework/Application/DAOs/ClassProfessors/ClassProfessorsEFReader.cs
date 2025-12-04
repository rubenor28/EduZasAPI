using System.Linq.Expressions;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.ClassProfessors;

public class ClassProfessorsEFReader(
    EduZasDotnetContext ctx,
    IMapper<ClassProfessor, ClassProfessorDomain> mapper
) : EFReader<UserClassRelationId, ClassProfessorDomain, ClassProfessor>(ctx, mapper)
{
    protected override Expression<Func<ClassProfessor, bool>> GetIdPredicate(
        UserClassRelationId id
    ) => cs => cs.ProfessorId == id.UserId && cs.ClassId == id.ClassId;
}
