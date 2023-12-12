using MLApiCore.Data;

namespace MLApiCore;

public abstract class TextMlClientBase
{
    /// <summary>
    /// Generates Data from ML Api. Result of generation will be written in input dictionary
    /// </summary>
    /// <param name="userId">User ID in database</param>
    /// <param name="input">input parameters for generation</param>
    /// <param name="onComplete">Returns generation status on complete</param>
    public abstract void Generate(Guid userId, Dictionary<string, string> input, Action<GenerationResult, Guid> onComplete);
}