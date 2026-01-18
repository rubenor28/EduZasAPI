using Application.DAOs;
using Application.DTOs.Answers;
using Application.Services.Validators;
using Application.UseCases.Common;

public class UpdateStudentTryStatusUseCase(
    IUpdaterAsync<AnswerIdDTO, AnswerUpdateDTO> updater,
    IReaderAsync<AnswerIdDTO, AnswerIdDTO> reader,
    IBusinessValidationService<AnswerUpdateDTO>? validator = null
) : UpdateUseCase<AnswerIdDTO, AnswerUpdateDTO, AnswerIdDTO>(updater, reader, validator)
{
    protected override AnswerIdDTO GetId(AnswerUpdateDTO dto)
    {
        throw new NotImplementedException();
    }
}
