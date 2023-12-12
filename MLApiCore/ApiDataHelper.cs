using MLApiCore.Data;

namespace MLApiCore;

public static class ApiDataHelper
{
    public static void AppendError(this Dictionary<string, string> input, string errorMessage)
    {
        input.TryAdd(ApiKeywords.ErrorMessage, errorMessage);
    }

    public static void AppendStatus(this Dictionary<string, string> input, GenerationResult result)
    {
        input.TryAdd(ApiKeywords.Status, result.ToString());
    }
}