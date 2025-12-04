using Application.DTOs.ClassResources;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.ClassResources;

public sealed class ClassResourceAssosiationEFQuerier(
    EduZasDotnetContext ctx,
    IEFProjector<Class, ClassResourceAssosiationDTO, ClassResourceAssosiationCriteriaDTO> projector,
    int maxPageSize
)
    : EFQuerier<ClassResourceAssosiationDTO, ClassResourceAssosiationCriteriaDTO, Class>(
        ctx,
        projector,
        maxPageSize
    )
{
    public override IQueryable<Class> BuildQuery(ClassResourceAssosiationCriteriaDTO criteria) =>
        _dbSet
            .AsNoTracking()
            .Where(c => c.ClassProfessors.Any(p => p.ProfessorId == criteria.ProfessorId))
            .OrderByDescending(c =>
                c.ClassProfessors.Where(cp => cp.ProfessorId == criteria.ProfessorId)
                    // Deberia haber solo 1 registro con profesor - clase unicos
                    // El uso de Max es unicamente para extraer el valor de la fecha
                    .Max(c => c.CreatedAt)
            );
}

// public sealed class ClassResourceAssosiationEFQuerier(EduZasDotnetContext ctx, int pageSize)
//     : IQuerierAsync<ClassResourceAssosiationDTO, ClassResourceAssosiationCriteriaDTO>
// {
//     private readonly EduZasDotnetContext _ctx = ctx;
//     private readonly int _pageSize = pageSize;
//     public int PageSize => _pageSize;
//
//     private int CalcOffset(int pageNumber)
//     {
//         if (pageNumber < 1)
//             pageNumber = 1;
//         return (pageNumber - 1) * _pageSize;
//     }
//
//     public async Task<
//         PaginatedQuery<ClassResourceAssosiationDTO, ClassResourceAssosiationCriteriaDTO>
//     > GetByAsync(ClassResourceAssosiationCriteriaDTO criteria)
//     {
//         var query = _ctx
//             .Classes.AsNoTracking()
//             .Where(c => c.ClassProfessors.Any(p => p.ProfessorId == criteria.ProfessorId))
//             .OrderByDescending(c =>
//                 c.ClassProfessors.Where(cp => cp.ProfessorId == criteria.ProfessorId)
//                     // Deberia haber solo 1 registro con profesor - clase unicos
//                     // El uso de Max es unicamente para extraer el valor de la fecha
//                     .Max(c => c.CreatedAt)
//             );
//
//         var totalRecords = await query.CountAsync();
//         var results = await query
//             .Select(c => new ClassResourceAssosiationDTO
//             {
//                 ClassId = c.ClassId,
//                 ClassName = c.ClassName,
//                 ResourceId = criteria.ResourceId,
//                 IsAssosiated = _ctx.ClassResources.Any(cr =>
//                     cr.ClassId == c.ClassId && cr.ResourceId == criteria.ResourceId
//                 ),
//             })
//             .Skip(CalcOffset(criteria.Page))
//             .Take(_pageSize)
//             .ToListAsync();
//
//         int totalPages = (int)Math.Ceiling((decimal)totalRecords / _pageSize);
//
//         return new()
//         {
//             Page = criteria.Page,
//             TotalPages = totalPages,
//             Criteria = criteria,
//             Results = results,
//         };
//     }
// }
