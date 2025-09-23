using EduZasAPI.Domain.Users;
using EduZasAPI.Domain.Common;
using EduZasAPI.Application.Common;
using EduZasAPI.Application.Users;
using EduZasAPI.Infraestructure.MinimalAPI.Application.Common;

namespace EduZasAPI.Infraestructure.MinimalAPI.Application.Users;

public static class UserCriteriaMAPIMapper
{
    public static Result<UserCriteriaDTO, List<FieldErrorDTO>> ToDomain(this UserCriteriaMAPI source)
    {
        var role = Optional<UserType>.None();
        var firstName = Optional<StringQueryDTO>.None();
        var midName = Optional<StringQueryDTO>.None();
        var fatherLastName = Optional<StringQueryDTO>.None();
        var motherLastname = Optional<StringQueryDTO>.None();
        var email = Optional<StringQueryDTO>.None();
        var password = Optional<StringQueryDTO>.None();

        var errs = new List<FieldErrorDTO>();
        StringQueryMAPIMapper.ParseStringQuery(source.FirstName, "FirstName", firstName, errs);
        StringQueryMAPIMapper.ParseStringQuery(source.MidName, "MidName", midName, errs);
        StringQueryMAPIMapper.ParseStringQuery(source.FatherLastName, "FatherLastName", fatherLastName, errs);
        StringQueryMAPIMapper.ParseStringQuery(source.MotherLastname, "MotherLastname", motherLastname, errs);
        StringQueryMAPIMapper.ParseStringQuery(source.Email, "Email", email, errs);
        StringQueryMAPIMapper.ParseStringQuery(source.Password, "Password", password, errs);

        if (errs.Count > 0)
            return Result.Err<UserCriteriaDTO, List<FieldErrorDTO>>(errs);

        return Result.Ok<UserCriteriaDTO, List<FieldErrorDTO>>(new UserCriteriaDTO
        {
            FirstName = firstName,
            MidName = midName,
            FatherLastName = fatherLastName,
            MotherLastname = motherLastname,
            Email = email,
            Password = password,
            Role = role,
            Page = source.Page,
            Active = source.Active.ToOptional(),
            CreatedAt = source.CreatedAt.ToOptional(),
            ModifiedAt = source.CreatedAt.ToOptional(),
        });
    }
}
