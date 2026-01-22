using Application.DAOs;
using Application.DTOs.Answers;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

public class FinishTryUseCase(
    IUpdaterAsync<AnswerDomain, AnswerUpdateDTO> updater,
    IReaderAsync<AnswerIdDTO, AnswerDomain> reader
) : IUseCaseAsync<AnswerIdDTO, Unit>
{
    private readonly IUpdaterAsync<AnswerDomain, AnswerUpdateDTO> _updater = updater;
    private readonly IReaderAsync<AnswerIdDTO, AnswerDomain> _reader = reader;

    public async Task<Result<Unit, UseCaseError>> ExecuteAsync(UserActionDTO<AnswerIdDTO> request)
    {
        var authorized = request.Executor.Role switch
        {
            UserType.ADMIN => true,
            _ => request.Executor.Id == request.Data.UserId,
        };

        if (!authorized)
            return UseCaseErrors.Unauthorized();

        var answer = await _reader.GetAsync(request.Data);
        if (answer is null)
            return UseCaseErrors.NotFound();

        await _updater.UpdateAsync(
            new()
            {
                ClassId = request.Data.ClassId,
                TestId = request.Data.TestId,
                UserId = request.Data.UserId,
                TryFinished = true,
            }
        );

        return Unit.Value;
    }
}
