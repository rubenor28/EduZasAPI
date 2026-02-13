using Application.DTOs.Classes;
using Application.DTOs.Contacts;
using Application.DTOs.Resources;
using Application.DTOs.Tests;
using Application.DTOs.Users;
using Application.Services.Validators;
using Domain.Entities.Questions;
using FluentValidationProj.Application.Services.Answers;
using FluentValidationProj.Application.Services.Answers.QuestionAnswers;
using FluentValidationProj.Application.Services.Auth;
using FluentValidationProj.Application.Services.Classes;
using FluentValidationProj.Application.Services.Common;
using FluentValidationProj.Application.Services.Contacts;
using FluentValidationProj.Application.Services.Resources;
using FluentValidationProj.Application.Services.Tests;
using FluentValidationProj.Application.Services.Tests.Questions;
using FluentValidationProj.Application.Services.Users;
using Microsoft.Extensions.DependencyInjection;

namespace Composition.Extensions;

/// <summary>
/// Métodos de extensión para registrar validadores en el contenedor de dependencias.
/// </summary>
internal static class ValidatorServiceCollectionExtensions
{
    /// <summary>
    /// Registra las implementaciones de validadores basados en <see cref="IBusinessValidationService{T}"/>.
    /// </summary>
    /// <param name="services">La colección de servicios donde se registrarán los validadores.</param>
    /// <returns>La colección de servicios con los validadores registrados.</returns>
    public static IServiceCollection AddValidators(this IServiceCollection services)
    {
        services.AddSingleton<IBusinessValidationService<ulong>, ULongFluentValidator>();
        services.AddSingleton<IBusinessValidationService<string>, EmailFluentValidator>();

        // Auth validators
        services.AddSingleton<IBusinessValidationService<UserCredentialsDTO>, UserCredentialsFluentValidator>();
        services.AddSingleton<IBusinessValidationService<NewUserDTO>, NewUserFluentValidator>();

        // User validators
        services.AddSingleton<IBusinessValidationService<UserUpdateDTO>, UserUpdateFluentValidator>();
        services.AddSingleton<IBusinessValidationService<UserCredentialsDTO>, UserCredentialsFluentValidator>();

        // Class validators
        services.AddSingleton<IBusinessValidationService<NewClassDTO>, NewClassFluentValidator>();
        services.AddSingleton<IBusinessValidationService<ClassUpdateDTO>, ClassUpdateFluentValidator>();

        // Contact validators
        services.AddSingleton<IBusinessValidationService<NewContactDTO>, NewContactFluentValidator>();
        services.AddSingleton<IBusinessValidationService<ContactUpdateDTO>, ContactUpdateFluentValidator>();

        // Resources validators
        services.AddSingleton<IBusinessValidationService<NewResourceDTO>, NewResourceFluentValidator>();
        services.AddSingleton<IBusinessValidationService<ResourceUpdateDTO>, ResourceUpdateFluentValidator>();

        // Test validators
        services.AddSingleton<IBusinessValidationService<NewTestDTO>, NewTestFluentValidator>();
        services.AddSingleton<IBusinessValidationService<TestUpdateDTO>, TestUpdateFluentValidator>();

        // Questions validator
        services.AddSingleton<IBusinessValidationService<ConceptRelationQuestion>, ConceptRelationFluentValidator>();
        services.AddSingleton<IBusinessValidationService<MultipleChoiseQuestion>, MultipleChoiseQuestionFluentValidator>();
        services.AddSingleton<IBusinessValidationService<MultipleSelectionQuestion>, MultipleSelectionQuestionFluentValidator>();
        services.AddSingleton<IBusinessValidationService<OpenQuestion>, OpenQuestionFluentValidator>();
        services.AddSingleton<IBusinessValidationService<OrderingQuestion>, OrderingQuestionFluentValidator>();

        // AnswerValidators
        services.AddSingleton<IAnswerUpdateStudentValidator, AnswerUpdateStudentFluentValidator>();

        // Question answer validators
        services.AddSingleton<IConceptRelationQuestionAnswerValidator, ConceptRelationQuestionAnswerFluentValidator>();
        services.AddSingleton<IMultipleChoiseQuestionAnswerValidator, MultipleChoiseQuestionAnswerFluentValidator>();
        services.AddSingleton<IMultipleSelectionQuestionAnswerValidator, MultipleSelectionQuestionAnswerFluentValidator>();
        services.AddSingleton<IOpenQuestionAnswerValidator, OpenQuestionAnswerFluentValidator>();
        services.AddSingleton<IOrderingQuestionAnswerValidator, OrderingQuestionAnswerFluentValidator>();

        return services;
    }
}
