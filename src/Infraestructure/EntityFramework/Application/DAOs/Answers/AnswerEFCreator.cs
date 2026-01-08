using Application.DTOs.Answers;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.Answers;

public sealed class AnswerEFCreator(
    EduZasDotnetContext ctx,
    IMapper<Answer, AnswerDomain> domainMapper,
    IMapper<AnswerIdDTO, Answer> newEntityMapper
) : EFCreator<AnswerDomain, AnswerIdDTO, Answer>(ctx, domainMapper, newEntityMapper);
