using MLApiCore.Data;

namespace MLApiCore;

public abstract class TextMlClientBase
{
    /// <summary>
    /// Generates Data from ML Api. Result of generation will be written in input dictionary
    /// </summary>
    /// <param name="userId">User ID in database</param>
    /// <param name="input">input parameters for generation</param>
    public abstract Task<GenerationResult> Generate(Guid userId, IReadOnlyDictionary<string, string?> input);
}