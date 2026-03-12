using System.Text.Json;
using System.Text.Json.Serialization;
using Domain.Entities.QuestionAnswers;
using Domain.Entities.Questions;

/// <summary>
/// Convertidor JSON personalizado para la interfaz <see cref="IQuestionAnswer"/>.
/// Permite la serialización y deserialización polimórfica de los distintos tipos de respuestas de preguntas.
/// </summary>
public class IQuestionAnswerJsonConverter : JsonConverter<IQuestionAnswer>
{
    /// <summary>
    /// Lee y convierte el JSON a un tipo derivado de <see cref="IQuestionAnswer"/>.
    /// </summary>
    public override IQuestionAnswer? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        // Creamos una copia del lector para poder inspeccionar el JSON sin consumirlo
        Utf8JsonReader readerClone = reader;

        // Parseamos el JSON a un DOM para poder navegarlo
        using var jsonDoc = JsonDocument.ParseValue(ref readerClone);
        var root = jsonDoc.RootElement;

        // Buscamos nuestra la propiedad que nos dice el tipo de pregunta.
        if (!root.TryGetProperty("type", out var typeProperty))
        {
            throw new JsonException(
                "El JSON de la pregunta no contiene la propiedad 'type' para determinar qué tipo de pregunta es."
            );
        }

        var typeName = typeProperty.GetString();

        // Obtenemos el texto JSON crudo del objeto actual
        var jsonText = root.GetRawText();

        // Basado en el tipo, delegamos al deserializador estándar para que cree la clase correcta.
        IQuestionAnswer? answer = typeName switch
        {
            QuestionTypes.MultipleChoise =>
                JsonSerializer.Deserialize<MultipleChoiseQuestionAnswer>(jsonText, options),
            QuestionTypes.MultipleSelection =>
                JsonSerializer.Deserialize<MultipleSelectionQuestionAnswer>(jsonText, options),
            QuestionTypes.Ordering => JsonSerializer.Deserialize<OrderingQuestionAnswer>(
                jsonText,
                options
            ),
            QuestionTypes.Open => JsonSerializer.Deserialize<OpenQuestionAnswer>(jsonText, options),
            QuestionTypes.ConceptRelation =>
                JsonSerializer.Deserialize<ConceptRelationQuestionAnswer>(jsonText, options),
            _ => throw new NotSupportedException(
                $"El tipo de respuesta '{typeName}' no es soportado."
            ),
        };

        // El lector original no ha avanzado. Para que el deserializador principal continúe,
        // debemos avanzar el lector saltando el objeto que acabamos de procesar manualmente.
        reader.Skip();

        return answer;
    }

    /// <summary>
    /// Escribe un objeto derivado de <see cref="IQuestionAnswer"/> como JSON.
    /// </summary>
    public override void Write(
        Utf8JsonWriter writer,
        IQuestionAnswer value,
        JsonSerializerOptions options
    )
    {
        writer.WriteStartObject();

        // Determinamos qué tipo de objeto es para escribir
        var typeName = value switch
        {
            MultipleChoiseQuestionAnswer => QuestionTypes.MultipleChoise,
            MultipleSelectionQuestionAnswer => QuestionTypes.MultipleSelection,
            OrderingQuestionAnswer => QuestionTypes.Ordering,
            OpenQuestionAnswer => QuestionTypes.Open,
            ConceptRelationQuestionAnswer => QuestionTypes.ConceptRelation,
            _ => throw new NotSupportedException(
                $"El tipo '{value.GetType()}' no puede ser serializado por IQuestionAnswerJsonConverter."
            ),
        };
        writer.WriteString("type", typeName);

        // Serializamos el objeto a un JsonDocument temporal para poder acceder a sus propiedades
        using var jsonDoc = JsonDocument.Parse(
            JsonSerializer.Serialize(value, value.GetType(), options)
        );

        // Copiamos todas las propiedades del objeto original al writer actual
        foreach (var property in jsonDoc.RootElement.EnumerateObject())
        {
            // Nos saltamos la propiedad 'type' si por alguna razón ya existiera
            if (property.Name.Equals("type", StringComparison.OrdinalIgnoreCase))
                continue;

            property.WriteTo(writer);
        }

        writer.WriteEndObject();
    }
}
