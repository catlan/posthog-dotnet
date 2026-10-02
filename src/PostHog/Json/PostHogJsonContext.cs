#if NET8_0_OR_GREATER
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using PostHog.Api;

namespace PostHog.Json;

/// <summary>
/// Source-generated serialization metadata for capturing events: the batch the SDK sends and the result it reads back.
/// </summary>
/// <remarks>
/// Used where the runtime can't generate code (Native AOT, see <see cref="JsonSerializerHelper"/>). There
/// reflection-based serialization is disabled, and the converter factories can't create their generic converters, so
/// every type that goes through the serializer has to be listed here. Event properties are typed <see cref="object"/>
/// and written by their runtime type: the common property value types are listed too.
/// </remarks>
[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true)]
// What /batch receives and returns.
[JsonSerializable(typeof(Dictionary<string, object>))]
[JsonSerializable(typeof(ReadOnlyCollection<CapturedEvent>))]
[JsonSerializable(typeof(IReadOnlyList<CapturedEvent>))]
[JsonSerializable(typeof(CapturedEvent))]
[JsonSerializable(typeof(ApiResult))]
[JsonSerializable(typeof(ApiErrorResult))]
// Property values.
[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(bool))]
[JsonSerializable(typeof(byte))]
[JsonSerializable(typeof(short))]
[JsonSerializable(typeof(int))]
[JsonSerializable(typeof(long))]
[JsonSerializable(typeof(uint))]
[JsonSerializable(typeof(ulong))]
[JsonSerializable(typeof(float))]
[JsonSerializable(typeof(double))]
[JsonSerializable(typeof(decimal))]
[JsonSerializable(typeof(DateTime))]
[JsonSerializable(typeof(DateTimeOffset))]
[JsonSerializable(typeof(TimeSpan))]
[JsonSerializable(typeof(Guid))]
[JsonSerializable(typeof(Uri))]
[JsonSerializable(typeof(JsonElement))]
[JsonSerializable(typeof(string[]))]
[JsonSerializable(typeof(int[]))]
[JsonSerializable(typeof(object[]))]
[JsonSerializable(typeof(List<string>))]
[JsonSerializable(typeof(List<object>))]
[JsonSerializable(typeof(IReadOnlyList<string>))]
[JsonSerializable(typeof(Dictionary<string, string>))]
[JsonSerializable(typeof(IReadOnlyDictionary<string, object>))]
internal sealed partial class PostHogJsonContext : JsonSerializerContext;

/// <summary>
/// <see cref="StringOrValue{T}"/> of <see cref="int"/> without reflection, for the source-generated
/// <see cref="PostHogJsonContext"/>: <see cref="StringOrValueConverter"/> makes its generic types at run time.
/// </summary>
internal sealed class StringOrInt32ValueConverter : JsonConverter<StringOrValue<int>>
{
    public override StringOrValue<int> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        reader.TokenType switch
        {
            JsonTokenType.String => reader.GetString() is { } stringValue ? new StringOrValue<int>(stringValue) : default,
            JsonTokenType.Null => default,
            _ => new StringOrValue<int>(reader.GetInt32()),
        };

    public override void Write(Utf8JsonWriter writer, StringOrValue<int> value, JsonSerializerOptions options)
    {
        if (value.IsString)
        {
            writer.WriteStringValue(value.StringValue);
        }
        else if (value.IsValue)
        {
            writer.WriteNumberValue(value.Value);
        }
        else
        {
            writer.WriteNullValue();
        }
    }
}
#endif
