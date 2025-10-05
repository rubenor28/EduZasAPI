using EduZasAPI.Application.Classes;
using EduZasAPI.Infraestructure.EntityFramework.Application.Common;
using EduZasAPI.Infraestructure.EntityFramework.Application.ClassStudents;
using Microsoft.EntityFrameworkCore;

namespace EduZasAPI.Infraestructure.EntityFramework.Application.Classes;

public class StudentPerClassEntityFrameworkRepository :
  CompositeKeyEFRepository<ClassUserRelationIdDTO, StudentClassRelationDTO, StudentClassRelationCriteriaDTO, ClassStudent>
{
    public StudentPerClassEntityFrameworkRepository(EduZasDotnetContext ctx, ulong pageSize) : base(ctx, pageSize) { }

    protected override StudentClassRelationDTO MapToDomain(ClassStudent efEntity) => new StudentClassRelationDTO
    {
        Id = new ClassUserRelationIdDTO { ClassId = efEntity.ClassId, UserId = efEntity.StudentId },
        Hidden = efEntity.Hidden
    };

    protected override ClassStudent NewToEF(StudentClassRelationDTO r) => new ClassStudent
    {
        ClassId = r.Id.ClassId,
        StudentId = r.Id.UserId
    };

    protected override IQueryable<ClassStudent> QueryFromCriteria(StudentClassRelationCriteriaDTO criteria) =>
      _ctx.ClassStudents.AsNoTracking().AsQueryable()
      .WhereOptional(criteria.UserId, userId => cr => cr.StudentId == userId)
      .WhereOptional(criteria.ClassId, classId => cr => cr.ClassId == classId);

    protected override ClassUserRelationIdDTO GetId(StudentClassRelationDTO entity) => entity.Id;

    protected async override Task<ClassStudent?> GetById(ClassUserRelationIdDTO id) =>
        await _ctx.ClassStudents
        .AsNoTracking()
        .AsQueryable()
        .Where(cs => cs.StudentId == id.UserId)
        .Where(cs => cs.ClassId == id.ClassId)
        .FirstAsync();

    protected async override Task<ClassStudent?> GetByIdTracked(ClassUserRelationIdDTO id) =>
        await _ctx.ClassStudents
        .AsTracking()
        .AsQueryable()
        .Where(cs => cs.StudentId == id.UserId)
        .Where(cs => cs.ClassId == id.ClassId)
        .FirstAsync();

    protected override void UpdateProperties(ClassStudent entity, StudentClassRelationDTO uProps)
    {
        entity.ClassId = uProps.Id.ClassId;
        entity.StudentId = uProps.Id.UserId;
    }

}
