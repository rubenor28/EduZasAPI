namespace EduZasAPI.Domain.ValueObjects.Common;

using EduZasAPI.Domain.Enums.Common;

/// <summary>
/// Representa una consulta de texto con un tipo específico de búsqueda.
/// </summary>
/// <remarks>
/// Esta estructura inmutable encapsula una cadena de texto y el tipo de búsqueda
/// que se debe aplicar. Admite cadenas vacías o que contengan solo espacios en blanco.
/// </remarks>
public readonly struct StringQuery
{
    /// <summary>
    /// Obtiene el texto de la consulta.
    /// </summary>
    /// <value>Cadena de texto a consultar. Puede estar vacía o contener espacios en blanco.</value>
    public string Text { get; }

    /// <summary>
    /// Obtiene el tipo de búsqueda a aplicar sobre el texto.
    /// </summary>
    /// <value>Tipo de búsqueda definido por <see cref="StringSearchType"/>.</value>
    public StringSearchType SearchType { get; }

    /// <summary>
    /// Inicializa una nueva instancia de la estructura <see cref="StringQuery"/>.
    /// </summary>
    /// <param name="text">Texto de la consulta. Puede estar vacío o contener espacios en blanco.</param>
    /// <param name="searchType">Tipo de búsqueda a aplicar.</param>
    /// <exception cref="ArgumentNullException">
    /// Se lanza cuando el texto es nulo.
    /// </exception>
    /// <remarks>
    /// Este constructor permite cadenas vacías o con espacios en blanco, pero no valores nulos.
    /// </remarks>
    public StringQuery(string text, StringSearchType searchType)
    {
        if (text == null) throw new ArgumentNullException("text can not be null", nameof(text));

        Text = text;
        SearchType = searchType;
    }

    /// <summary>
    /// Inicializa una nueva instancia de la estructura <see cref="StringQuery"/> con búsqueda por igualdad exacta.
    /// </summary>
    /// <param name="text">Texto de la consulta. Puede estar vacío o contener espacios en blanco.</param>
    /// <remarks>
    /// Este constructor establece automáticamente el tipo de búsqueda como <see cref="StringSearchType.EQ"/>.
    /// Permite cadenas vacías o con espacios en blanco, pero no valores nulos.
    /// </remarks>
    public StringQuery(string text) : this(text, StringSearchType.EQ) { }
}
