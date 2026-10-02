using System.Text;
using System.Text.Json;

namespace PostHog.Json;

internal static class JsonSerializerHelper
{
    internal static readonly JsonSerializerOptions Options = CreateOptions();

    static JsonSerializerOptions CreateOptions()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
        };
#if NET8_0_OR_GREATER
        // Native AOT can't serialize by reflection (it is off by default) or make the converter factories' generic
        // converters: the source-generated context covers capturing events, and System.Text.Json's own support for
        // read-only collections and dictionaries stands in for the factories.
        if (!System.Runtime.CompilerServices.RuntimeFeature.IsDynamicCodeSupported)
        {
            options.TypeInfoResolver = PostHogJsonContext.Default;
            options.Converters.Add(new StringOrInt32ValueConverter());
            return options;
        }
#endif
        options.Converters.Add(new ReadOnlyCollectionJsonConverterFactory());
        options.Converters.Add(new ReadOnlyDictionaryJsonConverterFactory());
        return options;
    }

    static readonly JsonSerializerOptions IndentedOptions = new(Options)
    {
        WriteIndented = true
    };
    public static async Task<string> SerializeToCamelCaseJsonStringAsync<T>(T obj, bool writeIndented = false)
    {
        var stream = await SerializeToCamelCaseJsonStreamAsync(obj, writeIndented);
        stream.Position = 0;
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);
        return Encoding.UTF8.GetString(memoryStream.ToArray());
    }

    static async Task<Stream> SerializeToCamelCaseJsonStreamAsync<T>(T obj, bool writeIndented = false)
    {
        var options = writeIndented ? IndentedOptions : Options;
        var stream = new MemoryStream();
        await JsonSerializer.SerializeAsync(stream, obj, options);
        return stream;
    }

    public static async Task<T?> DeserializeFromCamelCaseJsonStringAsync<T>(string json)
    {
        using var jsonStream = new MemoryStream(Encoding.UTF8.GetBytes(json));
        jsonStream.Position = 0;
        return await DeserializeFromCamelCaseJsonAsync<T>(jsonStream);
    }

    public static async Task<T?> DeserializeFromCamelCaseJsonAsync<T>(
        Stream jsonStream,
        CancellationToken cancellationToken = default) =>
        await JsonSerializer.DeserializeAsync<T>(jsonStream, Options, cancellationToken);
}

