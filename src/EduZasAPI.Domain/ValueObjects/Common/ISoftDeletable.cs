/// <summary>
/// Define un contrato para entidades que soportan eliminación lógica (soft delete).
/// </summary>
/// <remarks>
/// Esta interfaz se utiliza para marcar entidades que en lugar de ser eliminadas físicamente
/// de la base de datos, son marcadas como inactivas, permitiendo su recuperación posterior
/// y manteniendo la integridad referencial de los datos.
/// </remarks>
public interface ISoftDeletable
{
    /// <summary>
    /// Obtiene o establece el estado de activación de la entidad.
    /// </summary>
    /// <value>
    /// true si la entidad está activa y disponible; false si ha sido eliminada lógicamente.
    /// </value>
    /// <remarks>
    /// Cuando una entidad se marca como inactiva (Active = false), se considera
    /// eliminada lógicamente y normalmente será excluida de las consultas regulares.
    /// </remarks>
    bool? Active { get; set; }
}
