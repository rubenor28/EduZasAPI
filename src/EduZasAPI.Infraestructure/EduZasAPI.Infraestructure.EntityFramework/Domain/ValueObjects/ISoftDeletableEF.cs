namespace EduZasAPI.Infraestructure.EntityFramework.Domain.Common;

public interface ISoftDeletableEF
{
    bool? Active { get; set; }
}
