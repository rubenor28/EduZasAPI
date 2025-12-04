using System.Linq.Expressions;
using Application.DTOs.Classes;
using Domain.Entities;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.Classes;

public class ClassProjector : IEFProjector<Class, ClassDomain, ClassCriteriaDTO>
{
    public Expression<Func<Class, ClassDomain>> GetProjection(ClassCriteriaDTO _) =>
        ef =>
            new()
            {
                Id = ef.ClassId,
                Active = ef.Active ?? false,
                ClassName = ef.ClassName,
                Color = ef.Color ?? "#007bff",
                Subject = ef.Subject,
                Section = ef.Section,
                CreatedAt = ef.CreatedAt,
                ModifiedAt = ef.ModifiedAt,
            };
}
