using System.Text.Json;
using System.Text.Json.Serialization;

namespace LeaveManagementSystem.API.Converters;

/// <summary>
/// Custom JSON converter for DateTime that handles ISO 8601 format
/// </summary>
public class DateTimeJsonConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var dateString = reader.GetString();
            if (string.IsNullOrEmpty(dateString))
            {
                return default;
            }
            
            // Try standard DateTime parsing first (handles most ISO 8601 formats)
            if (DateTime.TryParse(dateString, null, System.Globalization.DateTimeStyles.RoundtripKind, out var date))
            {
                // Convert to UTC if not already
                if (date.Kind == DateTimeKind.Unspecified)
                {
                    date = DateTime.SpecifyKind(date, DateTimeKind.Utc);
                }
                return date.ToUniversalTime();
            }
            
            // Try parsing with specific ISO 8601 formats
            string[] formats = {
                "yyyy-MM-ddTHH:mm:ss.fffZ",
                "yyyy-MM-ddTHH:mm:ssZ",
                "yyyy-MM-ddTHH:mm:ss.fff",
                "yyyy-MM-ddTHH:mm:ss",
                "yyyy-MM-dd"
            };
            
            foreach (var format in formats)
            {
                if (DateTime.TryParseExact(dateString, format, null, System.Globalization.DateTimeStyles.AssumeUniversal | System.Globalization.DateTimeStyles.AdjustToUniversal, out var parsedDate))
                {
                    return parsedDate.ToUniversalTime();
                }
            }
        }
        else if (reader.TokenType == JsonTokenType.Null)
        {
            return default;
        }
        
        throw new JsonException($"Unable to convert to DateTime. Value: {reader.GetString()}");
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ"));
    }
}

