using System.Text.Json;
using System.Text.Json.Serialization;
using Domain.Entities.Questions;

namespace InterfaceAdapters.Mappers.Tests;

public class IQuestionJsonConverter : JsonConverter<IQuestion>
{
    public override IQuestion? Read(
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
        IQuestion? question = typeName switch
        {
            QuestionTypes.MultipleChoise => JsonSerializer.Deserialize<MultipleChoiseQuestion>(
                jsonText,
                options
            ),
            QuestionTypes.MultipleSelection =>
                JsonSerializer.Deserialize<MultipleSelectionQuestion>(jsonText, options),
            QuestionTypes.Ordering => JsonSerializer.Deserialize<OrderingQuestion>(
                jsonText,
                options
            ),
            QuestionTypes.Open => JsonSerializer.Deserialize<OpenQuestion>(jsonText, options),
            QuestionTypes.ConceptRelation => JsonSerializer.Deserialize<ConceptRelationQuestion>(
                jsonText,
                options
            ),
            _ => throw new NotSupportedException(
                $"El tipo de pregunta '{typeName}' no es soportado."
            ),
        };

        // El lector original no ha avanzado. Para que el deserializador principal continúe,
        // debemos avanzar el lector saltando el objeto que acabamos de procesar manualmente.
        reader.Skip();

        return question;
    }

    public override void Write(
        Utf8JsonWriter writer,
        IQuestion value,
        JsonSerializerOptions options
    )
    {
        writer.WriteStartObject();

        // Determinamos qué tipo de objeto es para escribir
        var typeName = value switch
        {
            MultipleChoiseQuestion => QuestionTypes.MultipleChoise,
            MultipleSelectionQuestion => QuestionTypes.MultipleSelection,
            OrderingQuestion => QuestionTypes.Ordering,
            OpenQuestion => QuestionTypes.Open,
            ConceptRelationQuestion => QuestionTypes.ConceptRelation,
            _ => throw new NotSupportedException(
                $"El tipo '{value.GetType()}' no puede ser serializado por IQuestionJsonConverter."
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
