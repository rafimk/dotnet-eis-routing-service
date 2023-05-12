using System.Text.Json;

namespace EisRoutingService.Utilities;

public class JsonSerializerUtil
{
    public static string SerializeEvent(object message)
    {
        var serializerOptions = new JsonSerializerOptions
        {
            Converters = { new DateTimeJsonBehaviour() },
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        string JsonString = JsonSerializer.Serialize(message, serializerOptions);
        return JsonString;
    }

    public static TOutput DeserializeObject<TOutput>(string message)
    {
        try
        {
            var serializerOptions = new JsonSerializerOptions
            {
                Converters = { new DateTimeJsonBehaviour() },
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            TOutput tOutput = JsonSerializer.Deserialize<TOutput>(message, serializerOptions)!;

            return tOutput;
        }
        catch
        {
            throw;
        }
    }

    public async static ValueTask<TOutput> DeserializeObjectAsync<TOutput>(string message, CancellationToken token)
    {

        var serializerOptions = new JsonSerializerOptions
        {
            Converters = { new DateTimeJsonBehaviour() },
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        try
        {
            byte[] byteArray = JsonSerializer.SerializeToUtf8Bytes(message);
            Stream memoryStream = new MemoryStream(byteArray);
            var awaitedComponent = await JsonSerializer.DeserializeAsync<TOutput>(memoryStream, serializerOptions, token);

            if (awaitedComponent == null)
            {
                Console.WriteLine("Payload is null");
            }
            return awaitedComponent!;
        }
        catch
        {
            await Console.Out.WriteLineAsync("Error occurred while converting to JSON");
            throw;
        }
    }
}

