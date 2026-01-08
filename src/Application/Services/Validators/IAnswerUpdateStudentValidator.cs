using Application.Services.Validators;
using Domain.Entities;

public interface IAnswerUpdateStudentValidator
    : IBusinessValidationService<(AnswerUpdateStudentDTO, TestDomain)>;
