using System.Text.Json;
using System.Text.Json.Serialization;
using ResRoomApi.Models;

namespace ResRoomApi.Helpers.Json;

public class ReservationStatusConverter : JsonConverter<ReservationStatus>
{
    public override ReservationStatus Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var enumString = reader.GetString();
            if (Enum.TryParse<ReservationStatus>(enumString, true, out var status))
                return status;
        }
        else if (reader.TokenType == JsonTokenType.Number)
        {
            if (reader.TryGetInt32(out int enumValue) && Enum.IsDefined(typeof(ReservationStatus), enumValue))
                return (ReservationStatus)enumValue;
        }

        throw new JsonException($"Invalid status value. Allowed values are: {string.Join(", ", Enum.GetNames(typeof(ReservationStatus)))}.");
    }

    public override void Write(Utf8JsonWriter writer, ReservationStatus value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString().ToLower());
    }
}