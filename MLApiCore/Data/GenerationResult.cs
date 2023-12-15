namespace MLApiCore.Data;

public readonly struct GenerationResult
{
    public readonly GenerationStatus Status;
    public readonly Dictionary<string, string?> Output;
    public readonly Guid SenderId;

    public GenerationResult(GenerationStatus status, Dictionary<string, string?> output, Guid senderId)
    {
        Status = status;
        Output = output;
        SenderId = senderId;
    }
}