using System.Linq.Expressions;
using Application.DTOs.ClassProfessors;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.ClassProfessors;

public sealed class ClassProfessorSummaryProjector
    : IEFProjector<User, ClassProfessorSummaryDTO, ClassProfessorSummaryCriteriaDTO>
{
    public Expression<Func<User, ClassProfessorSummaryDTO>> GetProjection(
        ClassProfessorSummaryCriteriaDTO criteria
    ) =>
        ef =>
            new(
                UserId: ef.UserId,
                Alias: ef.AgendaContactContacts.Where(ac =>
                        ac.UserId == ef.UserId && ac.AgendaOwnerId == criteria.ProfessorId
                    )
                    .Select(ac => ac.Alias)
                    .FirstOrDefault(),
                FirstName: ef.FirstName,
                MidName: ef.MidName,
                FatherLastName: ef.FatherLastname,
                MotherLastname: ef.MotherLastname,
                Owner: ef.ClassProfessors.Where(cp =>
                        cp.ClassId == criteria.ClassId && cp.ProfessorId == criteria.ProfessorId
                    )
                    .Select(cp => cp.IsOwner)
                    .FirstOrDefault()
                ?? false
            );
}
