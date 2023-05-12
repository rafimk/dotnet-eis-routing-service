using System.Globalization;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace EisRoutingService.Utilities;

public class DateTimeJsonBehaviour : JsonConverter<DateTime>
{
    private readonly string dateFormat = "dd-MM-yyyy hh:mm:ss";

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        DateTime.ParseExact(reader.GetString()!, dateFormat, CultureInfo.InvariantCulture);

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options) => writer.WriteStringValue(value.ToString(dateFormat, CultureInfo.InvariantCulture));

}
