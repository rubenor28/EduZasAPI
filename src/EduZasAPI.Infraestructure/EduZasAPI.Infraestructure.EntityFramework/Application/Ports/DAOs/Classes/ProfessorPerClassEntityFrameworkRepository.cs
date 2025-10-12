using EduZasAPI.Application.Classes;
using EduZasAPI.Infraestructure.EntityFramework.Application.Common;
using EduZasAPI.Infraestructure.EntityFramework.Application.ClassProfessors;
using Microsoft.EntityFrameworkCore;

namespace EduZasAPI.Infraestructure.EntityFramework.Application.Classes;

public class ProfessorPerClassEntityFrameworkRepository :
  CompositeKeyEFRepository<ClassUserRelationIdDTO, ProfessorClassRelationDTO, ProfessorClassRelationCriteriaDTO, ClassProfessor>
{
    public ProfessorPerClassEntityFrameworkRepository(EduZasDotnetContext ctx, ulong pageSize) : base(ctx, pageSize) { }

    protected override ProfessorClassRelationDTO MapToDomain(ClassProfessor efEntity) => new ProfessorClassRelationDTO
    {
        Id = new ClassUserRelationIdDTO { ClassId = efEntity.ClassId, UserId = efEntity.ProfessorId },
        IsOwner = efEntity.IsOwner ?? false
    };

    protected override ClassProfessor NewToEF(ProfessorClassRelationDTO r) => new ClassProfessor
    {
        ClassId = r.Id.ClassId,
        ProfessorId = r.Id.UserId,
        IsOwner = r.IsOwner
    };

    protected override IQueryable<ClassProfessor> QueryFromCriteria(ProfessorClassRelationCriteriaDTO criteria) =>
      _ctx.ClassProfessors.AsNoTracking().AsQueryable()
      .WhereOptional(criteria.UserId, userId => cr => cr.ProfessorId == userId)
      .WhereOptional(criteria.ClassId, classId => cr => cr.ClassId == classId)
      .WhereOptional(criteria.IsOwner, isOwner => cr => cr.IsOwner == isOwner);

    protected override ClassUserRelationIdDTO GetId(ProfessorClassRelationDTO entity) => entity.Id;

    protected async override Task<ClassProfessor?> GetById(ClassUserRelationIdDTO id) =>
        await _ctx.ClassProfessors
        .AsNoTracking()
        .AsQueryable()
        .Where(cs => cs.ProfessorId == id.UserId)
        .Where(cs => cs.ClassId == id.ClassId)
        .FirstOrDefaultAsync();

    protected async override Task<ClassProfessor?> GetByIdTracked(ClassUserRelationIdDTO id) =>
        await _ctx.ClassProfessors
        .AsTracking()
        .AsQueryable()
        .Where(cs => cs.ProfessorId == id.UserId)
        .Where(cs => cs.ClassId == id.ClassId)
        .FirstOrDefaultAsync();

    protected override void UpdateProperties(ClassProfessor entity, ProfessorClassRelationDTO uProps)
    {
        entity.ClassId = uProps.Id.ClassId;
        entity.ProfessorId = uProps.Id.UserId;
        entity.IsOwner = uProps.IsOwner;
    }
}
