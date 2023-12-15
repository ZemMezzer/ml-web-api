using MLApiCore.Data;
using Newtonsoft.Json;

namespace MLApiCore;

public static class ApiDataExtensions
{
    public static void AppendError(this Dictionary<string, string?> input, string? errorMessage)
    {
        input.TryAdd(ApiKeywords.ErrorMessage, errorMessage);
    }

    public static void AppendStatus(this Dictionary<string, string?> input, GenerationStatus status)
    {
        input.TryAdd(ApiKeywords.Status, status.ToString());
    }

    public static string ToJson(this Dictionary<string, string?> input) => JsonConvert.SerializeObject(input);
}